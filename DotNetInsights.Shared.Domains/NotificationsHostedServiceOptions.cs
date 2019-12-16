using System;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public sealed class NotificationsHostedServiceOptions
    {
        private NotificationsHostedServiceOptions(Action<NotificationsHostedServiceOptions> optionsAction)
        {
            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

            optionsAction(this);
        }

        public int PollingInterval { get; set; }
        public int ProcessingInterval { get; set; }

        public static NotificationsHostedServiceOptions Create(Action<NotificationsHostedServiceOptions> optionsAction)
        {
            return new NotificationsHostedServiceOptions(optionsAction);
        }
    }
}