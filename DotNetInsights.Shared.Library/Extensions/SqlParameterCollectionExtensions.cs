using Microsoft.Data.SqlClient;
using System;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class SqlParameterCollectionExtensions
    {
        public static SqlParameterCollection Add(this SqlParameterCollection collection, string parameterName, object value)
        {
            if(value == null)
                value = DBNull.Value;

            collection.AddWithValue(parameterName, value);
            return collection;
        }
    }
}