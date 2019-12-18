using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public sealed class NotificationsHostedService 
        : AsyncQueueServiceBase<NotificationSubscriberQueueItem, NotificationsHostedServiceOptions>, IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var isEnabled = _logger.IsEnabled(LogLevel.Information);
            _logger.LogInformation("Starting Notification hosted service...");
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await FlushQueue();
            _logger.LogInformation("Stopping Notification hosted service...");
        }

        public override async Task ProcessQueueItem(NotificationSubscriberQueueItem queueItem)
        {
            await queueItem.NotificationSubscriber
                    .OnChangeAsync(queueItem.Item)
                    .ConfigureAwait(false);

        }

        public NotificationsHostedService(ILogger<NotificationsHostedService> logger, NotificationsHostedServiceOptions notificationsHostedServiceOptions, ConcurrentQueue<NotificationSubscriberQueueItem> notificationSubscriberQueue) 
            : base(logger, notificationSubscriberQueue, notificationsHostedServiceOptions)
        {
            _logger = logger;
            _notificationsHostedServiceOptions = notificationsHostedServiceOptions;
            _notificationSubscriberQueue = notificationSubscriberQueue;

        }

        private readonly ILogger<NotificationsHostedService> _logger;
        private readonly NotificationsHostedServiceOptions _notificationsHostedServiceOptions;
        private readonly ConcurrentQueue<NotificationSubscriberQueueItem> _notificationSubscriberQueue;
    }
}