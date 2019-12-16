using DotNetInsights.Shared.Contracts;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services
{
    public sealed class DefaultNotificationUnsubscriber : INotificationUnsubscriber
    {
        private readonly IList<INotificationSubscriber> _notificationSubscribers;
        private readonly INotificationSubscriber _notificationSubscriber;

        public DefaultNotificationUnsubscriber(IList<INotificationSubscriber> notificationSubscribers, INotificationSubscriber notificationSubscriber)
        {
            _notificationSubscribers = notificationSubscribers;
            _notificationSubscriber = notificationSubscriber;
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        public void Unsubscribe()
        {
            if(_notificationSubscribers.Contains(_notificationSubscriber))
                _notificationSubscribers.Remove(_notificationSubscriber);
        }
    }
}
