
using DotNetInsights.Shared.Library;
using DotNetInsights.Shared.Library.Options;
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
            await Task.CompletedTask;
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Final flush of notification queue...");
            await DisposeAsync(true);
            _logger.LogInformation("Stopping Notification hosted service...");
        }

        public override async Task ProcessQueueItem(NotificationSubscriberQueueItem queueItem)
        {
            await queueItem.NotificationSubscriber
                    .OnChangeAsync(queueItem.Item)
                    .ConfigureAwait(false);
        }

        public NotificationsHostedService(ILogger<NotificationsHostedService> logger, NotificationsHostedServiceOptions notificationsHostedServiceOptions, ConcurrentQueue<NotificationSubscriberQueueItem> notificationSubscriberQueue, IServiceProvider serviceProvider) 
            : base(logger, serviceProvider, notificationSubscriberQueue, notificationsHostedServiceOptions, true)
        {
            _logger = logger;

        }

        private readonly ILogger<NotificationsHostedService> _logger;
    }
}