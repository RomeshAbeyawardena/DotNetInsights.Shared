﻿using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public class SqlDependencyHostedService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _dependencyManagerTimer = new Timer(async(state) => await Listen(state), 
                null, _sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);

            _queueTimer = new Timer(async(state) => await ProcessQueue(state), 
                null, _sqlDependencyHostedServiceOptions.PollingInterval, Timeout.Infinite);
            
            _serviceScope = _serviceProvider.CreateScope();
            _sqlDependencyManager = _serviceScope.ServiceProvider.GetRequiredService<ISqlDependencyManager>();

            _sqlDependencyHostedServiceOptions
                .ConfigureSqlDependencyManager?.Invoke(_sqlDependencyManager);

            _sqlDependencyManager.OnChange += _sqlDependencyManager_OnChange;
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
            if(!_sqlDependencyHostedServiceChangeEventQueue.IsEmpty 
                && _sqlDependencyHostedServiceChangeEventQueue.TryDequeue(out var queueItem)){
                await queueItem.SqlDependencyChangeEvent.OnChange(queueItem.CommandEntry);

                _queueTimer.Change(_sqlDependencyHostedServiceChangeEventQueue.IsEmpty 
                    ? _sqlDependencyHostedServiceOptions.PollingInterval
                    : _sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);
            }

            _queueTimer.Change(_sqlDependencyHostedServiceOptions.PollingInterval, Timeout.Infinite);
        }

        private async Task Listen(object state)
        {
            await _sqlDependencyManager.Listen();
            _dependencyManagerTimer.Change(_sqlDependencyHostedServiceOptions.ProcessingInterval, Timeout.Infinite);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _queueTimer.Dispose();
            _dependencyManagerTimer.Dispose();
            _serviceScope?.Dispose();
        }

        public SqlDependencyHostedService(IServiceProvider serviceProvider, 
            ConcurrentQueue<SqlDependencyChangeEventQueueItem> sqlDependencyHostedServiceChangeEventQueue,
            SqlDependencyHostedServiceOptions sqlDependencyHostedServiceOptions)
        {
            _serviceProvider = serviceProvider;
            _sqlDependencyHostedServiceChangeEventQueue = sqlDependencyHostedServiceChangeEventQueue;
            _sqlDependencyHostedServiceOptions =  sqlDependencyHostedServiceOptions;
        }

        private Timer _queueTimer;
        private ISqlDependencyManager _sqlDependencyManager;
        private Timer _dependencyManagerTimer;
        private IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<SqlDependencyChangeEventQueueItem> _sqlDependencyHostedServiceChangeEventQueue;
        private readonly SqlDependencyHostedServiceOptions _sqlDependencyHostedServiceOptions;
    }
}