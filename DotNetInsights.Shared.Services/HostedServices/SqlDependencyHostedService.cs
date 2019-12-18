using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public class SqlDependencyHostedService : AsyncQueueServiceBase<SqlDependencyChangeEventQueueItem, SqlDependencyHostedServiceOptions>, IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SqlDependency hosted service...");
            _serviceScope = _serviceProvider.CreateScope();
            _sqlDependencyManager = _serviceScope.ServiceProvider.GetRequiredService<ISqlDependencyManager>();

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

            var requestedGenericService = (ISqlDependencyChangeEvent) _serviceScope
                .ServiceProvider.GetRequiredService(requestGenericService);

            if(requestedGenericService == null)
                return;

            _sqlDependencyHostedServiceChangeEventQueue
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
            
            await FlushQueue();
            
            _sqlDependencyManager.Stop(_connectionString);
        }


        protected override async Task Dispose(bool gc)
        {
            await base.Dispose(gc);

            _dependencyManagerTimer?.Dispose();
            _serviceScope?.Dispose();
        }

        public override async Task ProcessQueueItem(SqlDependencyChangeEventQueueItem queueItem)
        {
            await queueItem.SqlDependencyChangeEvent.OnChange(queueItem.CommandEntry);
        }

        public SqlDependencyHostedService(ILogger<SqlDependencyHostedService> logger, IServiceProvider serviceProvider, 
            ConcurrentQueue<SqlDependencyChangeEventQueueItem> sqlDependencyHostedServiceChangeEventQueue,
            SqlDependencyHostedServiceOptions sqlDependencyHostedServiceOptions) : base(logger, sqlDependencyHostedServiceChangeEventQueue, sqlDependencyHostedServiceOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sqlDependencyHostedServiceChangeEventQueue = sqlDependencyHostedServiceChangeEventQueue;
            _sqlDependencyHostedServiceOptions =  sqlDependencyHostedServiceOptions;
            _connectionString = _sqlDependencyHostedServiceOptions.ConfigureConnectionString(_serviceProvider);
        }

        private readonly string _connectionString;
        private ISqlDependencyManager _sqlDependencyManager;
        private Timer _dependencyManagerTimer;
        private IServiceScope _serviceScope;
        private readonly ILogger<SqlDependencyHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<SqlDependencyChangeEventQueueItem> _sqlDependencyHostedServiceChangeEventQueue;
        private readonly SqlDependencyHostedServiceOptions _sqlDependencyHostedServiceOptions;
    }
}