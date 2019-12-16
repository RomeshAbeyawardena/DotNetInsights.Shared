using DotNetInsights.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public abstract class DefaultNotificationSubscriber<TEvent> : INotificationSubscriber<TEvent>
    {
        public Type NotificationType => typeof(TEvent);

        public abstract void OnChange(TEvent @event);

        public void OnChange(object @event)
        {
            OnChange((TEvent)@event);
        }

        public abstract Task OnChangeAsync(TEvent @event);

        public async Task OnChangeAsync(object @event)
        {
            await OnChangeAsync((TEvent)@event).ConfigureAwait(false);
        }

        protected IServiceScope GetScopeService(IServiceProvider serviceProvider)
        {
            return serviceProvider.CreateScope();
        }
    }
}
