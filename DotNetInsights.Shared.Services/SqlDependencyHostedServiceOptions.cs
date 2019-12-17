using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Domains;
using System;

namespace DotNetInsights.Shared.Services
{
    public class SqlDependencyHostedServiceOptions
    {
        protected SqlDependencyHostedServiceOptions(Action<SqlDependencyHostedServiceOptions> action)
        {
            action?.Invoke(this);
        }

        public Action<ISqlDependencyManager> ConfigureSqlDependencyManager { get; set; }
        public int PollingInterval { get; set; }
        public int ProcessingInterval { get; set; }

        public static SqlDependencyHostedServiceOptions Create(Action<SqlDependencyHostedServiceOptions> action)
        {
            return new SqlDependencyHostedServiceOptions(action);
        }

        
    }
}