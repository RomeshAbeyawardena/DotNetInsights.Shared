using DotNetInsights.Shared.Contracts;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public abstract class DefaultEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        public abstract Task<TEvent> Push(TEvent @event);
        
        public virtual async Task<TEvent> Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var foundCommand = GetCommand(command.Name);

            if(foundCommand == null)
                throw new MethodAccessException();

            return await foundCommand(command).ConfigureAwait(false);
        }

        public async Task<IEvent> Push(IEvent @event)
        {
            return await Push((TEvent)@event).ConfigureAwait(false);
        }
        
        protected ISwitch<string, Func<ICommand, Task<TEvent>>> CommandSwitch;
        protected virtual Func<ICommand, Task<TEvent>> GetCommand(string commandName) => CommandSwitch.Case(commandName);

        async Task<IEvent> IEventHandler.Send<TCommand>(TCommand command)
        {
            return await Send(command).ConfigureAwait(false);
        }

        protected DefaultEventHandler()
        {
            CommandSwitch = DefaultSwitch
                .Create<string, Func<ICommand, Task<TEvent>>>();
        }
    }
}
