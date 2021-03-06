﻿using Microsoft.Extensions.DependencyInjection;

using DotNetInsights.Shared.Contracts.Providers;
using DotNetInsights.Shared.Services.Providers;
using System;
using DotNetInsights.Shared.Library.Options;

namespace DotNetInsights.Shared.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDefaultEntityValueProvider<TEntity>(this IServiceCollection services, 
            Action<IDefaultEntityValueProvider<TEntity>> defaultEntityProviderRegistration = null)
            where TEntity: class
        {
            var defaultEntityProvider = DefaultEntityValueProvider.Create<TEntity>();
            defaultEntityProviderRegistration?.Invoke(defaultEntityProvider);
            return services.AddSingleton(defaultEntityProvider);
        }

        public static IServiceCollection ConfigureHostedServiceOptions(this IServiceCollection services, Action<HostedServiceOptions> optionsAction)
        {
            optionsAction(HostedServiceOptions.Create(services));
            return services;
        }
    }
}
