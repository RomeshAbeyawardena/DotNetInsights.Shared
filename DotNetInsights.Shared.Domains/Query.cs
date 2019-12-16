using DotNetInsights.Shared.Domains.Enumerations;
using System.Data;

namespace DotNetInsights.Shared.Domains
{
    public class Query
    {
        protected Query()
        {
            
        }

        public static Query CreateQuery(object value, DbType? dbType = null, 
            QueryCondition queryCondition = QueryCondition.And, QueryType queryType = QueryType.Equal, int groupByOrder = 0)
        {
            if (!dbType.HasValue)
                dbType = System.Data.DbType.Object;

            return new Query
            {
                Condition = queryCondition,
                DbType = dbType,
                Type = queryType,
                Value = value,
                GroupByOrder = groupByOrder
            };
        }

        public int GroupByOrder { get; set; }
        public QueryCondition Condition { get; set; }
        public QueryType Type { get; set; }
        public DbType? DbType { get; set; }
        public object Value { get; set; }
    }
}