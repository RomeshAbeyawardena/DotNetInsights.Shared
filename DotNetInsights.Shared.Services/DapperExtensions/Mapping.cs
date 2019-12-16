using Dapper;
using DotNetInsights.Shared.Contracts.DapperExtensions;
using DotNetInsights.Shared.Domains;
using DotNetInsights.Shared.Domains.Enumerations;
using DotNetInsights.Shared.Services.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.DapperExtensions
{
    public class Mapping<T> : IMapping<T>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;

        public Mapping(IFormatProvider formatProvider, IDbConnection dbConnection, string schema, string name)
        {
            _formatProvider = formatProvider;
            _dbConnection = dbConnection;
            SchemaName = schema;
            TableName = name;
            GetQuery = "FROM " + GenerateSqlMapping();
        }
        public string GetQuery { get; }
        public IEnumerable<string> Columns { get; private set; }

        public IQueryParameters<T> GetDefaultQueryParameters(T entity)
        {
            var queryParameters = CreateQueryParameters();

            return KeyProperties.Aggregate(queryParameters, (current, keyProperty) =>
                current.AddQueryParameter(keyProperty.Name, keyProperty.GetValue(entity)));
        }

        public IEnumerable<PropertyInfo> KeyProperties { get; private set; }
        public PropertyInfo IdentityProperty { get; private set; }
        public IDictionary<string, CustomAttributeData> MappedColumns { get; private set; }

        public string SchemaName { get; }
        public string TableName { get; }

        public IChangeSet<T> GetChanges(T entity, IQueryParameters<T> queryParameters = null)
        {
            if (queryParameters == null)
                queryParameters = GetDefaultQueryParameters(entity);

            var foundEntry = FirstOrDefault(queryParameters);
            var updatableProperties = Properties.Where(column => !IdentityProperty.Name.Equals(column.Name, StringComparison.InvariantCulture)).ToArray();
            return new MappingChangeSet<T>(updatableProperties, foundEntry, entity, queryParameters);
        }

        public T FirstOrDefault(IQueryParameters<T> queryParameters)
        {
            var sqlQuery = queryParameters.GetSqlQuery(out var dynamicParameters);
            var query = GenerateSqlQuery(sqlQuery, 1);

            return _dbConnection.QuerySingle<T>(query, dynamicParameters);
        }

        public IQueryParameters<T> CreateQueryParameters(IDictionary<string, Query> queryParameterDictionary = null)
        {
            return new QueryParameters<T>(this, queryParameterDictionary);
        }

        public Task<IEnumerable<T>> ToList(IQueryParameters<T> queryParameters = null, int topLimit = 0)
        {
            var dynamicParameters = new DynamicParameters();
            var sqlQuery = queryParameters?.GetSqlQuery(out dynamicParameters);

            var query = GenerateSqlQuery(sqlQuery, topLimit);
            return _dbConnection.QueryAsync<T>(query, dynamicParameters);
        }

        public bool Any(IQueryParameters<T> queryParameters = null)
        {
            var dynamicParameters = new DynamicParameters();
            var sqlQuery = queryParameters?.GetSqlQuery(out dynamicParameters);

            var query = GenerateAggregateSqlQuery(sqlQuery, AggregateType.Count);

            return _dbConnection.QuerySingle<int>(query, dynamicParameters) > 0;
        }

        public T Save(T entity)
        {
            if(!KeyProperties.Any())
                throw new ArgumentException("Unable to save entity, primary key(s) are not defined");

            var queryParameters = GetDefaultQueryParameters(entity);

            var any = Any(queryParameters);

            var resultSet = any
                ? Update(entity, queryParameters)
                : Insert(entity);

            return entity;
        }

        public async Task<int> Remove(T entity)
        {
            var queryParameters = CreateQueryParameters();

            queryParameters = KeyProperties.Aggregate(queryParameters, (current, keyProperty) => 
                current.AddQueryParameter(keyProperty.Name, keyProperty.GetValue(entity)));

            var queryClause = queryParameters.GetSqlQuery(out var dynamicParameters);

            var query = $"DELETE FROM {GetSchemaTable()} {queryClause}";

            return await _dbConnection.ExecuteAsync(query, dynamicParameters, _dbTransaction).ConfigureAwait(false);
        }

        public IDbTransaction GetDbTransaction()
        {
            _dbTransaction = _dbTransaction ?? _dbConnection.BeginTransaction();
            
            return _dbTransaction;
        }

        public void Rollback()
        {
            _dbTransaction?.Rollback();
            _dbTransaction?.Dispose();
        }

        public void Commit()
        {
            _dbTransaction?.Commit();
            _dbTransaction?.Dispose();
        }

        public int Remove(IQueryParameters<T> removeQueryParameters)
        {
            var queryClause = removeQueryParameters.GetSqlQuery(out var dynamicParameters);

            var query = $"DELETE FROM {GetSchemaTable()} {queryClause}";
            return _dbConnection.Execute(query, dynamicParameters, _dbTransaction);
        }

        private int Update(T value, IQueryParameters<T> queryParameters)
        {
            var changes = GetChanges(value, queryParameters);

            if (changes.ChangeState == ChangeState.Unchanged)
                return 0;

            var setList = changes.ChangeSetDictionary.Where(changeSet => changeSet.Value == ChangeState.Modified)
                .Select(changeSet => string.Format(_formatProvider, "[{0}] = @{0}", changeSet.Key.Name));

            var queryClause = queryParameters.GetSqlQuery(out var dynamicParameters);

            var query = $"UPDATE {GetSchemaTable()} SET {string.Join(", ", setList)} {queryClause}";

            Console.WriteLine(query);

            return _dbConnection.Execute(query, value, _dbTransaction);
        }

        private int Insert(T value)
        {
            var insertList = Columns.Where(column => !IdentityProperty.Name.Equals(column, StringComparison.InvariantCulture)).ToArray();
            var itemsList = string.Join(", @", insertList);
            var insertQuery = $"INSERT INTO {GetSchemaTable()} ({GetCommaSeparatedList(insertList)}) VALUES (@{itemsList})";
            Console.WriteLine(insertQuery);
            return _dbConnection.Execute(insertQuery, value, _dbTransaction);
        }

        private string GetCommaSeparatedList(IEnumerable<string> columnList, string separator = ", ")
        {
            return string.Join(separator, columnList.Select(column => $"[{column}]"));
        }

        private static IEnumerable<string> GetProperties(IEnumerable<PropertyInfo> properties)
        {
            return properties
                .Where(property => property.GetCustomAttribute(typeof(NotMappedAttribute)) == null)
                .Select(property => property.Name);
        }

        private string GenerateAggregateSqlQuery(string sqlQuery, AggregateType aggregateType)
        {
            var finalQuery = string.Format(_formatProvider, "SELECT {0}({1}) {2} {3}",
                aggregateType.ToString().ToUpper(), IdentityProperty.Name, GetQuery, sqlQuery);

            Console.WriteLine(finalQuery);

            return finalQuery;
        }

        private string GenerateSqlQuery(string sqlQuery = "", int topLimit = 0)
        {
            var finalQuery = string.Format(_formatProvider, "SELECT {0}{1} {2} {3}",
                topLimit > 0 ? $"TOP {topLimit} " : "",
                GetCommaSeparatedList(Columns), GetQuery, sqlQuery);

            Console.WriteLine(finalQuery);

            return finalQuery;
        }

        private string GenerateSqlMapping()
        {
            var tSourceType = typeof(T);
            var columnList = new List<string>();

            Properties = tSourceType.GetProperties();

            MappedColumns = Properties.Where(property => property.CustomAttributes.Any(customAttribute =>
                customAttribute.AttributeType == typeof(ColumnAttribute))).ToDictionary(a => a.Name,
                a => a.CustomAttributes.FirstOrDefault(b => b.AttributeType == typeof(ColumnAttribute)));

            KeyProperties = Properties.Where(property => property.CustomAttributes.Any(customAttribute =>
                customAttribute.AttributeType == typeof(KeyAttribute))).ToArray();

            IdentityProperty = Properties.SingleOrDefault(property => property.CustomAttributes.Any(customAttribute =>
                customAttribute.AttributeType == typeof(IdentityAttribute))) ?? KeyProperties.FirstOrDefault();

            foreach (var property in GetProperties(Properties))
            {
                var propertyName = property;

                if (MappedColumns.ContainsKey(propertyName))
                {
                    var constructorArguments = MappedColumns[propertyName].ConstructorArguments;

                    if (constructorArguments.Any())
                    {
                        propertyName = constructorArguments.SingleOrDefault().Value.ToString();
                    }
                }

                columnList.Add(propertyName);
            }

            Columns = columnList;

            return GetSchemaTable(tSourceType);
        }

        private string GetSchemaTable(Type sourceType = null)
        {
            if(sourceType == null)
                sourceType = typeof(T);

            return string.Format(_formatProvider, "[{0}].[{1}]",
                SchemaName,
                string.IsNullOrEmpty(TableName) ? sourceType.Name : TableName);
        }

        private IEnumerable<PropertyInfo> Properties { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool gc)
        {
            _dbTransaction?.Dispose();
        }

        public bool IsValid(out DbException exception)
        {
            try
            { 
                exception = null;
                return Any();
            }
            catch(DbException dbException)
            {
                exception = dbException;
                return false;
            }
        }

        public async Task<IEnumerable<T>> ToList(Func<IQueryParameters<T>, IQueryParameters<T>> queryParameterAction, int topLimit = 0)
        {
            var queryParameters = CreateQueryParameters(new Dictionary<string, Query>());
            queryParameters = queryParameterAction?.Invoke(queryParameters);
            return await ToList(queryParameters).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> whereExpression)
        {
            
            return await ToList().ConfigureAwait(false);
        }
        
    }
}
