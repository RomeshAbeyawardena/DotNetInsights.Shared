using Microsoft.Extensions.DependencyInjection;
using System;
using DotNetInsights.Shared.Services.HostedServices;

namespace DotNetInsights.Shared.Services
{
    public sealed class HostedServiceOptions
    {
        private readonly IServiceCollection _services;

        public HostedServiceOptions ConfigureNotifications(Action<NotificationsHostedServiceOptions> optionsAction)
        {
            _services
                .AddSingleton(NotificationsHostedServiceOptions.Create(optionsAction));
            return this;
        }

        public HostedServiceOptions ConfigureSqlDependency(Action<SqlDependencyHostedServiceOptions> optionsAction)
        {
            _services
                .AddSingleton(SqlDependencyHostedServiceOptions.Create(optionsAction));
            return this;
        }

        public HostedServiceOptions ConfigureSqlLoggerOptions(Action<SqlLoggerOptions> configure)
        {
            _services.AddSingleton(SqlLoggerOptions.Create(configure));
            return this;
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
