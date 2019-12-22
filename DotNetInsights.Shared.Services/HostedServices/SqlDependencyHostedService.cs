using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotNetInsights.Shared.Library.Options;
using DotNetInsights.Shared.Library;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public class SqlDependencyHostedService : AsyncQueueHandlerServiceBase<SqlDependencyChangeEventQueueItem, SqlDependencyHostedServiceOptions>, IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SqlDependency hosted service...");
            _sqlDependencyManager = ServiceScope.ServiceProvider.GetRequiredService<ISqlDependencyManager>();

            _sqlDependencyHostedServiceOptions
                .ConfigureSqlDependencyManager?.Invoke(_sqlDependencyManager);

            _sqlDependencyManager.OnChange += sqlDependencyManager_OnChange;
            
            await _sqlDependencyManager.Start(_connectionString);
            _dependencyManagerTimer = new Timer(async(state) => await Listen(state), 
                null, _sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);

        }

        private void sqlDependencyManager_OnChange(object sender, Domains.CommandEntrySqlNotificationEventArgs e)
        {
            var service = typeof(ISqlDependencyChangeEvent<>); 

            if(e.CommandEntry == null)
                return;

            var requestGenericService = service.MakeGenericType(e.CommandEntry.EntityType);

            var requestedGenericService = (ISqlDependencyChangeEvent) ServiceScope
                .ServiceProvider.GetRequiredService(requestGenericService);

            if(requestedGenericService == null)
                return;

            _sqlDependencyChangeEventQueue
                .Enqueue(SqlDependencyChangeEventQueueItem
                    .Create(requestedGenericService, e));
        }

        private async Task Listen(object state)
        {
            _logger.LogInformation("Listening for database changes...");
            await _sqlDependencyManager.Listen();
            _logger.LogInformation("Sql Dependency change event triggered.");
            _dependencyManagerTimer.Change(_sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping SqlDependency hosted service...");
            await Task.CompletedTask;
            
            await DisposeAsync(true);
            
            _sqlDependencyManager.Stop(_connectionString);
        }


        protected override async Task DisposeAsync(bool gc)
        {
            await base.DisposeAsync(gc);

            _dependencyManagerTimer?.Dispose();
        }

        public override async Task ProcessQueueItem(SqlDependencyChangeEventQueueItem queueItem)
        {
            await queueItem.SqlDependencyChangeEvent.OnChange(queueItem.CommandEntry);
        }

        public SqlDependencyHostedService(ILogger<SqlDependencyHostedService> logger, IServiceProvider serviceProvider, 
            SqlDependencyChangeEventQueue sqlDependencyChangeEventQueue,
            SqlDependencyHostedServiceOptions sqlDependencyHostedServiceOptions) : base(logger, 
                serviceProvider, sqlDependencyChangeEventQueue.Queue, sqlDependencyHostedServiceOptions, true)
        {
            _logger = logger;
            _sqlDependencyChangeEventQueue = sqlDependencyChangeEventQueue;
            _sqlDependencyHostedServiceOptions =  sqlDependencyHostedServiceOptions;
            _connectionString = _sqlDependencyHostedServiceOptions.ConfigureConnectionString(serviceProvider);
        }

        private readonly string _connectionString;
        private ISqlDependencyManager _sqlDependencyManager;
        private Timer _dependencyManagerTimer;
        private readonly ILogger<SqlDependencyHostedService> _logger;
        private readonly SqlDependencyChangeEventQueue _sqlDependencyChangeEventQueue;
        private readonly SqlDependencyHostedServiceOptions _sqlDependencyHostedServiceOptions;
    }
}