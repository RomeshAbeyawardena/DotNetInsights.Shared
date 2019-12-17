using System;
using System.Threading.Tasks;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetInsights.Shared.WebApp.Handlers
{
    public class TestSubscriber : DefaultNotificationSubscriber<IEvent<Test>>
    {
        private IMyScopedService _myScopedService;
        private readonly IServiceProvider _serviceProvider;

        public override void OnChange(IEvent<Test> @event)
        {
            throw new System.NotImplementedException();
        }

        public override async Task OnChangeAsync(IEvent<Test> @event)
        {
            using (var myscope = this.GetScopeService(_serviceProvider))
            {
                _myScopedService = myscope.ServiceProvider.GetRequiredService<IMyScopedService>();
               _myScopedService.Execute();
            }
            
        }

        public TestSubscriber(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}