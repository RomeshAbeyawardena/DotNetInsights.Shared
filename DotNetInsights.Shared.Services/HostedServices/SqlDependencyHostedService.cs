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
    public class SqlDependencyHostedService : IHostedService, IDisposable
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SqlDependency hosted service...");
            _serviceScope = _serviceProvider.CreateScope();
            _sqlDependencyManager = _serviceScope.ServiceProvider.GetRequiredService<ISqlDependencyManager>();

            _sqlDependencyHostedServiceOptions
                .ConfigureSqlDependencyManager?.Invoke(_sqlDependencyManager);

            _sqlDependencyManager.OnChange += _sqlDependencyManager_OnChange;
            await _sqlDependencyManager.Start(_connectionString);
            _dependencyManagerTimer = new Timer(async(state) => await Listen(state), 
                null, _sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);

            _queueTimer = new Timer(async(state) => await ProcessQueue(state), 
                null, _sqlDependencyHostedServiceOptions.PollingInterval, Timeout.Infinite);
            
        }

        private void _sqlDependencyManager_OnChange(object sender, Domains.CommandEntrySqlNotificationEventArgs e)
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

        private async Task ProcessQueue(object state)
        {
            _logger.LogInformation("Polling queue...");
            if(_sqlDependencyHostedServiceChangeEventQueue.IsEmpty)
            {
                _logger.LogDebug("Queue empty - polling mode");
                _queueTimer.Change(_sqlDependencyHostedServiceOptions.PollingInterval, Timeout.Infinite);
            }
            else if(_sqlDependencyHostedServiceChangeEventQueue.TryDequeue(out var queueItem))
            {
                _logger.LogInformation("Processing queue item - processing mode");
                await queueItem.SqlDependencyChangeEvent.OnChange(queueItem.CommandEntry);

                _queueTimer.Change(_sqlDependencyHostedServiceChangeEventQueue.IsEmpty 
                    ? _sqlDependencyHostedServiceOptions.PollingInterval
                    : _sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);
            }
            else
               _queueTimer.Change(_sqlDependencyHostedServiceOptions.PollingInterval, Timeout.Infinite);
        }

        private async Task Listen(object state)
        {
            _logger.LogInformation("Listening for database changes...");
            await _sqlDependencyManager.Listen();
            _logger.LogInformation("Sql Dependency change event triggered.");
            _dependencyManagerTimer.Change(_sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);
        }

        private async Task FlushQueue()
        {
            _logger.LogInformation("Flushing queue...");
            while(!_sqlDependencyHostedServiceChangeEventQueue.IsEmpty 
                && _sqlDependencyHostedServiceChangeEventQueue.TryDequeue(out var queueItem))
                await queueItem.SqlDependencyChangeEvent.OnChange(queueItem.CommandEntry);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping SqlDependency hosted service...");
            await Task.CompletedTask;
            
            await FlushQueue();
            
            _sqlDependencyManager.Stop(_connectionString);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool gc)
        {
            _queueTimer?.Dispose();
            _dependencyManagerTimer?.Dispose();
            _serviceScope?.Dispose();
        }

        public SqlDependencyHostedService(ILogger<SqlDependencyHostedService> logger, IServiceProvider serviceProvider, 
            ConcurrentQueue<SqlDependencyChangeEventQueueItem> sqlDependencyHostedServiceChangeEventQueue,
            SqlDependencyHostedServiceOptions sqlDependencyHostedServiceOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sqlDependencyHostedServiceChangeEventQueue = sqlDependencyHostedServiceChangeEventQueue;
            _sqlDependencyHostedServiceOptions =  sqlDependencyHostedServiceOptions;
            _connectionString = _sqlDependencyHostedServiceOptions.ConfigureConnectionString(_serviceProvider);
        }

        private readonly string _connectionString;
        private Timer _queueTimer;
        private ISqlDependencyManager _sqlDependencyManager;
        private Timer _dependencyManagerTimer;
        private IServiceScope _serviceScope;
        private readonly ILogger<SqlDependencyHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<SqlDependencyChangeEventQueueItem> _sqlDependencyHostedServiceChangeEventQueue;
        private readonly SqlDependencyHostedServiceOptions _sqlDependencyHostedServiceOptions;
    }
}