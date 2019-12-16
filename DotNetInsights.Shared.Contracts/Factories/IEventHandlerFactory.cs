using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Factories
{
    public interface IEventHandlerFactory
    {
        IEventHandler<TEvent> GetEventHandler<TEvent>() where TEvent : IEvent;
        Task<TEvent> Push<TEvent>(TEvent notifyEvent) where TEvent : IEvent;
        Task<TEvent> Send<TEvent, TCommand>(TCommand command) 
            where TEvent : IEvent
            where TCommand : ICommand;
    }
}
