using Microsoft.Extensions.DependencyInjection;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public class DefaultNotificationHandlerFactory : INotificationHandlerFactory
    {
        public void Notify<TEvent>(TEvent @event)
        {
            GetNotificationHandler<TEvent>().Notify(@event);
        }

        public INotificationUnsubscriber Subscribe<TEvent>(INotificationSubscriber<TEvent> notificationSubscriber)
        {
            return GetNotificationHandler<TEvent>().Subscribe(notificationSubscriber);
        }

        private INotificationHandler<IEvent> GetNotificationHandler<TEvent>()
        {
            var eventType = typeof(TEvent);

            var genericEventType = typeof(IEvent);

            var notificationHandlerType = typeof(INotificationHandler<>)
                .MakeGenericType(genericEventType);

            return _serviceProvider.GetService(notificationHandlerType) as INotificationHandler<IEvent>;
        }

        public async Task NotifyAsync<TEvent>(TEvent @event)
        {
            var notificationHandler = GetNotificationHandler<TEvent>();
            await notificationHandler.NotifyAsync(@event).ConfigureAwait(false);
        }

        public DefaultNotificationHandlerFactory(IServiceProvider serviceProvider, 
            IList<Type> registeredNotificationTypes, IList<INotificationUnsubscriber> notificationUnsubscribers)
        {
            _serviceProvider = serviceProvider;
            _registeredNotificationTypes = registeredNotificationTypes;
            _notificationUnsubscribers = notificationUnsubscribers;

            SubscribeToAllNotificationEvents();
        }

        private void SubscribeToAllNotificationEvents()
        {
            foreach (var subscriberEventType in _registeredNotificationTypes)
            {
                
                var subscriberEvent = _serviceProvider.GetRequiredService(subscriberEventType);
                var genericArgs = subscriberEventType.GetGenericArguments();

                var factoryType = GetType();
                
                var unsubscriber = factoryType.GetMethod("Subscribe")
                    .MakeGenericMethod(genericArgs)
                    .Invoke(this, new object [] {
                        subscriberEvent
                });

                Console.WriteLine("{0} Subscribed", subscriberEventType.FullName);

                _notificationUnsubscribers.Add((INotificationUnsubscriber)unsubscriber);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool a)
        {
            foreach (var notificationUnsubscriber in _notificationUnsubscribers)
            {
                Console.WriteLine("{0} Unsubscribed", notificationUnsubscriber);
                notificationUnsubscriber.Dispose();
            }

            _notificationUnsubscribers.Clear();
            GC.SuppressFinalize(this);
        }

        private readonly IList<INotificationUnsubscriber> _notificationUnsubscribers;
        private readonly IList<Type> _registeredNotificationTypes;
        private readonly IServiceProvider _serviceProvider;
    }
}
