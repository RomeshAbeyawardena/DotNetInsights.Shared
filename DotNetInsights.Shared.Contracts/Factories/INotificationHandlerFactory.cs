using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Factories
{
    public interface INotificationHandlerFactory : IDisposable
    {
        INotificationUnsubscriber Subscribe<TEvent>(INotificationSubscriber<TEvent> notificationSubscriber);
        void Notify<TEvent>(TEvent notifyEvent);
        Task NotifyAsync<TEvent>(TEvent notifyEvent);
    }
}
