using DotNetInsights.Shared.Domains;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.DapperExtensions
{
    public interface IMapping<T> : IDisposable
    {
        string SchemaName { get; }
        string TableName { get; }
        string GetQuery { get; }
        IEnumerable<string> Columns { get; }
        IQueryParameters<T> GetDefaultQueryParameters(T entity);
        IEnumerable<PropertyInfo> KeyProperties { get; }
        PropertyInfo IdentityProperty{ get; }

        IQueryParameters<T> CreateQueryParameters(IDictionary<string, Query> queryParameterDictionary = null);
        IChangeSet<T> GetChanges(T entity, IQueryParameters<T> queryParameters = null);
        T FirstOrDefault(IQueryParameters<T> queryParameters = null);
        Task<IEnumerable<T>> ToList(IQueryParameters<T> queryParameters = null, int topLimit = 0);
        Task<IEnumerable<T>> ToList(Func<IQueryParameters<T>, IQueryParameters<T>> queryParameters, int topLimit = 0);
        Task<IEnumerable<T>> Where(Expression<Func<T, bool>> whereExpression);
        bool Any(IQueryParameters<T> queryParameters = null);
        int Remove(IQueryParameters<T> removeQueryParameters);
        T Save(T entity);
        Task<int> Remove(T entity);
        IDbTransaction GetDbTransaction();

        bool IsValid(out DbException exception);
        void Rollback();
        void Commit();
    }
}