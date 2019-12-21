using System;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Library.Options
{
    public sealed class SqlLoggerOptions : IAsyncQueueServiceOptions
    {
        private SqlLoggerOptions(Action<SqlLoggerOptions> configure)
        {
            configure(this);
        }

        public Func<IServiceProvider, string> GetConnectionString { get; set; }
        public Func<IServiceProvider, string> GetTableSchema { get; set; }
        public Func<IServiceProvider, string> GetTableName { get; set; }
        public Func<IServiceProvider, string> GetLogOptionsTableName { get; set; }
        
        public int PollingInterval { get; set; }
        public int ProcessingInterval { get; set; }

        public static SqlLoggerOptions Create(Action<SqlLoggerOptions> configure)
        {
            return new SqlLoggerOptions(configure);
        }
    }
}