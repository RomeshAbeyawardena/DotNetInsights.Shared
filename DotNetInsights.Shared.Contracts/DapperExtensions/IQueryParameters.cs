using Dapper;
using DotNetInsights.Shared.Domains;
using DotNetInsights.Shared.Domains.Enumerations;
using System.Data;

namespace DotNetInsights.Shared.Contracts.DapperExtensions
{
    public interface IQueryParameters<T>
    {
        IQueryParameters<T> AddQueryParameter(string name, object value,
            QueryType queryType = QueryType.Equal,
            QueryCondition queryCondition = QueryCondition.And,
            DbType? dbType = null, int groupByOrder = 0);

        string GetSqlQuery(out DynamicParameters dynamicParameters);
    }
}