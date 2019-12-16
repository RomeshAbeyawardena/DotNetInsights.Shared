using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public sealed class NotificationSubscriberQueueItem
    {
        private NotificationSubscriberQueueItem(INotificationSubscriber notificationSubscriber, object item)
        {
            NotificationSubscriber = notificationSubscriber;
            Item = item;
        }

        public static NotificationSubscriberQueueItem Create(INotificationSubscriber notificationSubscriber, object item)
        {
            return new NotificationSubscriberQueueItem(notificationSubscriber, item);
        }

        public INotificationSubscriber NotificationSubscriber { get; }
        public object Item { get; }
    }
}