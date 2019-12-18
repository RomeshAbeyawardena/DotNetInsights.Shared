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
        : AsyncQueueHandlerServiceBase<NotificationSubscriberQueueItem, NotificationsHostedServiceOptions>, IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Notification hosted service...");
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Final flush of notification queue...");
            await Dispose(true);
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

        }

        private readonly ILogger<NotificationsHostedService> _logger;
    }
}