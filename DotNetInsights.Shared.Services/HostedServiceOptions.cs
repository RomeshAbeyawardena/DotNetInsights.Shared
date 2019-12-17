using Microsoft.Extensions.DependencyInjection;
using System;
using DotNetInsights.Shared.Services.HostedServices;

namespace DotNetInsights.Shared.Services
{
    public sealed class HostedServiceOptions
    {
        private readonly IServiceCollection _services;

        public IServiceCollection ConfigureNotifications(Action<NotificationsHostedServiceOptions> optionsAction)
        {
            return _services.AddSingleton(NotificationsHostedServiceOptions.Create(optionsAction));
        }

        public IServiceCollection ConfigureSqlDependency(Action<SqlDependencyHostedServiceOptions> optionsAction)
        {
            return _services.AddSingleton(SqlDependencyHostedServiceOptions.Create(optionsAction));
        }

        private HostedServiceOptions(IServiceCollection services)
        {
            _services = services;
        }

        public static HostedServiceOptions Create(IServiceCollection services)
        {
            return new HostedServiceOptions(services);
        }


    }
}
