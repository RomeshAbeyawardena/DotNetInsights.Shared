using System;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Services
{
    public sealed class SqlLoggerOptions
    {
        private SqlLoggerOptions(Action<SqlLoggerOptions> configure)
        {
            configure(this);
        }
        public Func<IServiceProvider, string> GetConnectionString { get; set; }
        public Func<IServiceProvider, string> GetTableSchema { get; set; }
        public Func<IServiceProvider, string> GetTableName { get; set; }

        public static SqlLoggerOptions Create(Action<SqlLoggerOptions> configure)
        {
            return new SqlLoggerOptions(configure);
        }
    }
}