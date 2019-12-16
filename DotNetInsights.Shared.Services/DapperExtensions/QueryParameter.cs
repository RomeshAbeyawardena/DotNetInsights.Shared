using Dapper;
using DotNetInsights.Shared.Contracts.DapperExtensions;
using DotNetInsights.Shared.Domains;
using DotNetInsights.Shared.Domains.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNetInsights.Shared.Services.DapperExtensions
{
    public class QueryParameters<T>: IQueryParameters<T>
    {
        private readonly IDictionary<string, Query> _queryParameterDictionary;

        public QueryParameters(IMapping<T> context, IDictionary<string, Query> queryParameterDictionary = null)
        {
            Context = context;
            _queryParameterDictionary = queryParameterDictionary 
                                        ?? new Dictionary<string, Query>();
        }

        public IQueryParameters<T> AddQueryParameter(string name, object value, 
            QueryType queryType = QueryType.Equal,
            QueryCondition queryCondition = QueryCondition.And, 
            DbType? dbType = null, int groupByOrder = 0)
        {
            if(_queryParameterDictionary.ContainsKey(name))
                throw new ArgumentException($"Query parameter with the name: {name} already exists", nameof(name));

            if(!Context.Columns.Contains(name))
                throw new ArgumentException($"Query parameter with the name: {name} does not exist in {Context.SchemaName}.{Context.TableName}", nameof(name));

            _queryParameterDictionary.Add(name,
                Query.CreateQuery(value, dbType, queryCondition, queryType, groupByOrder));
            return this;
        }

        public string GetSqlQuery(out DynamicParameters dynamicParameters)
        {
            dynamicParameters = new DynamicParameters();
            var isFirstParameter = true;
            var querySql = "WHERE ";
            foreach (var queryParameter in _queryParameterDictionary)
            {
                var query = queryParameter.Value;

                if (isFirstParameter) isFirstParameter = false;
                else querySql += $" {query.Condition.ToString().ToUpper()} ";

                switch (queryParameter.Value.Type)
                {
                    case QueryType.Equal:
                        querySql += $"[{queryParameter.Key}] = @{queryParameter.Key.ToLower()}";
                        break;

                    case QueryType.LessThan:
                        querySql += $"[{queryParameter.Key}] < @{queryParameter.Key.ToLower()}";
                        break;
                    case QueryType.GreaterThan:
                        querySql += $"[{queryParameter.Key}] > @{queryParameter.Key.ToLower()}";
                        break;
                    case QueryType.LessThanOrEqual:
                        querySql += $"[{queryParameter.Key}] <= @{queryParameter.Key.ToLower()}";
                        break;
                    case QueryType.GreaterThanOrEqual:
                        querySql += $"[{queryParameter.Key}] >= @{queryParameter.Key.ToLower()}";
                        break;
                }
                

                dynamicParameters.Add(queryParameter.Key, queryParameter.Value.Value, 
                    queryParameter.Value.DbType);
            }

            return querySql;
        }

        public IMapping<T> Context { get; }
    }
}