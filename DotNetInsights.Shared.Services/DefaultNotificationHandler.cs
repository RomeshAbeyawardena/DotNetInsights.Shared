using DotNetInsights.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using DotNetInsights.Shared.Services.HostedServices;

namespace DotNetInsights.Shared.Services
{
    public sealed class DefaultNotificationHandler<TEvent> : INotificationHandler<TEvent>
    {
        public void Notify(TEvent @event)
        {
            foreach(var notificationSubscriber in _notificationSubscribersList)
            {
                _notificationSubscriberQueue
                    .Enqueue(NotificationSubscriberQueueItem.Create(notificationSubscriber, @event));
            }
        }

        public void Notify(object @event)
        {
            Notify((TEvent)@event);
        }

        public INotificationUnsubscriber Subscribe(INotificationSubscriber<TEvent> notificationSubscriber)
        {
            return Subscribe((INotificationSubscriber)notificationSubscriber);
        }

        public INotificationUnsubscriber Subscribe(INotificationSubscriber notificationSubscriber)
        {
            if(!_notificationSubscribersList.Contains(notificationSubscriber))
                _notificationSubscribersList.Add(notificationSubscriber);

            return new DefaultNotificationUnsubscriber(_notificationSubscribersList, notificationSubscriber);
        }

        public async Task NotifyAsync(TEvent @event)
        {
            foreach(var notificationSubscriber in _notificationSubscribersList)
            {
                var eventType = @event.GetType();
                var notificationType = notificationSubscriber.NotificationType;

                var t = eventType.GetGenericArguments();
                var m = notificationType.GetGenericArguments();

                Console.WriteLine("{0}: {1}", eventType, notificationType);

                if(t.All(ty => m.Contains(ty)))
                    _notificationSubscriberQueue.Enqueue(NotificationSubscriberQueueItem.Create(notificationSubscriber, @event));
            }
        }

        public async Task NotifyAsync(object @event)
        {
            await NotifyAsync((TEvent)@event).ConfigureAwait(false);
        }

        public DefaultNotificationHandler(ConcurrentQueue<NotificationSubscriberQueueItem> notificationSubscriberQueue)
        {
            _notificationSubscriberQueue = notificationSubscriberQueue;
            _notificationSubscribersList = new List<INotificationSubscriber>();
        }

        private readonly ConcurrentQueue<NotificationSubscriberQueueItem> _notificationSubscriberQueue;
        private readonly IList<INotificationSubscriber> _notificationSubscribersList;
    }
}
