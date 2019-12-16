using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts
{
    public interface IEventHandler
    {
        Task<IEvent> Push(IEvent @event);
        Task<IEvent> Send<TCommand>(TCommand command)
            where TCommand : ICommand;
    }

    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : IEvent
    {
        Task<TEvent> Push(TEvent @event);
        new Task<TEvent> Send<TCommand>(TCommand command)
            where TCommand : ICommand;
    }
}
