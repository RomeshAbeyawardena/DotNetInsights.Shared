using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public class DefaultMediator : IMediator
    {
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly INotificationHandlerFactory _notificationHandlerFactory;

        public void Notify<TEvent>(TEvent @event)
        {
            _notificationHandlerFactory.Notify(@event);
        }

        public async Task NotifyAsync<TEvent>(TEvent @event)
        {
            await _notificationHandlerFactory.NotifyAsync(@event).ConfigureAwait(false);
        }

        public async Task<TEvent> Push<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            return await _eventHandlerFactory.Push(@event).ConfigureAwait(false);
        }

        public async Task<TEvent> Send<TEvent, TCommand>(TCommand command) 
            where TEvent : IEvent
            where TCommand : ICommand
        {
            return await _eventHandlerFactory.Send<TEvent,TCommand>(command).ConfigureAwait(false);
        }

        public Task<TEvent> Send<TEvent>(ICommand command) where TEvent : IEvent
        {
            return Send<TEvent, ICommand>(command);
        }

        public DefaultMediator(IEventHandlerFactory eventHandlerFactory, INotificationHandlerFactory notificationHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _notificationHandlerFactory = notificationHandlerFactory;
        }
    }
}
