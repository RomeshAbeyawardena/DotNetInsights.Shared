using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            var isEnabled = _logger.IsEnabled(LogLevel.Information);
            _logger.LogInformation("Starting Notification hosted service...");
            _backgroundTaskTimer = new Timer(async (state) => await ProcessQueue(state)
                .ConfigureAwait(false), null, _notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
        }

        private async Task ProcessQueue(object state)
        {
            _logger.LogInformation("Polling queue...");
            if (_notificationSubscriberQueue.IsEmpty)
            {
                _logger.LogDebug("Queue empty - polling mode");
                _backgroundTaskTimer.Change(_notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
            }
            else if (_notificationSubscriberQueue.TryDequeue(out var queueitem))
            {
                _logger.LogInformation("Processing queue item - processing mode");
                await queueitem.NotificationSubscriber
                    .OnChangeAsync(queueitem.Item)
                    .ConfigureAwait(false);

                _backgroundTaskTimer.Change(_notificationSubscriberQueue.Count > 0
                        ? _notificationsHostedServiceOptions.ProcessingInterval
                        : _notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
            }
            else
                _backgroundTaskTimer.Change(_notificationsHostedServiceOptions.PollingInterval, Timeout.Infinite);
        }

        private async Task FlushQueue()
        {
            _logger.LogInformation("Flushing queue...");
            while (!_notificationSubscriberQueue.IsEmpty
                    && _notificationSubscriberQueue.TryDequeue(out var queueitem))
                await queueitem.NotificationSubscriber
                        .OnChangeAsync(queueitem.Item)
                        .ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Notification hosted service...");
            await FlushQueue();
        }

        public void Dispose()
        {
            _backgroundTaskTimer?.Dispose();
        }

        public NotificationsHostedService(ILogger<NotificationsHostedService> logger, NotificationsHostedServiceOptions notificationsHostedServiceOptions, ConcurrentQueue<NotificationSubscriberQueueItem> notificationSubscriberQueue)
        {
            _logger = logger;
            _notificationsHostedServiceOptions = notificationsHostedServiceOptions;
            _notificationSubscriberQueue = notificationSubscriberQueue;

        }

        private Timer _backgroundTaskTimer;
        private readonly ILogger<NotificationsHostedService> _logger;
        private readonly NotificationsHostedServiceOptions _notificationsHostedServiceOptions;
        private readonly ConcurrentQueue<NotificationSubscriberQueueItem> _notificationSubscriberQueue;
    }
}