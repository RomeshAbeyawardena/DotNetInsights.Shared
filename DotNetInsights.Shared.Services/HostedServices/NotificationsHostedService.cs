using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public sealed class NotificationsHostedService : IHostedService, IDisposable
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _backgroundTaskTimer = new Timer(async(state) => await Work(state)
                .ConfigureAwait(false), null, _notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
        }

        private async Task Work(object state)
        {
                if(!_notificationSubscriberQueue.IsEmpty 
                    && _notificationSubscriberQueue.TryDequeue(out var queueitem))
                {
                    await queueitem.NotificationSubscriber
                        .OnChangeAsync(queueitem.Item)
                        .ConfigureAwait(false);
                    
                    _backgroundTaskTimer.Change(_notificationSubscriberQueue.Count > 0 
                            ? _notificationsHostedServiceOptions.ProcessingInterval 
                            : _notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
                    return;
                }
            
                _backgroundTaskTimer.Change(_notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            while(!_notificationSubscriberQueue.IsEmpty 
                    && _notificationSubscriberQueue.TryDequeue(out var queueitem))
                await queueitem.NotificationSubscriber
                        .OnChangeAsync(queueitem.Item)
                        .ConfigureAwait(false);
        }

        public void Dispose()
        {
            _backgroundTaskTimer.Dispose();
        }

        public NotificationsHostedService(NotificationsHostedServiceOptions notificationsHostedServiceOptions, ConcurrentQueue<NotificationSubscriberQueueItem> notificationSubscriberQueue)
        {
            _notificationsHostedServiceOptions = notificationsHostedServiceOptions;
            _notificationSubscriberQueue = notificationSubscriberQueue;
            
        }

        private Timer _backgroundTaskTimer;
        
        private readonly NotificationsHostedServiceOptions _notificationsHostedServiceOptions;
        private readonly ConcurrentQueue<NotificationSubscriberQueueItem> _notificationSubscriberQueue;
    }
}