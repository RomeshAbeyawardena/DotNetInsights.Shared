using Microsoft.Extensions.DependencyInjection;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public class DefaultEventHandlerFactory : IEventHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public IEventHandler<TEvent> GetEventHandler<TEvent>()
            where TEvent : IEvent
        {
            return (IEventHandler<TEvent>) _serviceProvider
                .GetRequiredService(typeof(IEventHandler<TEvent>));
        }

        public async Task<TEvent> Push<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            var eventHandler = GetEventHandler<TEvent>();
            return await eventHandler.Push(@event).ConfigureAwait(false);
        }

        public async Task<TEvent> Send<TEvent, TCommand>(TCommand command)
            where TEvent : IEvent
            where TCommand : ICommand
        {
            var eventHandler = GetEventHandler<TEvent>();
            return await eventHandler.Send(command).ConfigureAwait(false);
        }

        public DefaultEventHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}
