using Microsoft.EntityFrameworkCore;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Providers;
using System;

namespace DotNetInsights.Shared.Services.Providers
{
    public static class DefaultEntityValueProvider
    {
        public static IDefaultEntityValueProvider<TEntity> Create<TEntity>()
            where TEntity : class
        {
            return DefaultEntityProvider<TEntity>.Create();
        }
    }

    public class DefaultEntityProvider<TEntity> : IDefaultEntityValueProvider<TEntity>
        where TEntity : class
    {
        public Action<IServiceProvider, TEntity> GetDefaultAssignAction(EntityState entityState)
        {
            if(defaultEntitySwitch.ContainsKey(entityState))
                return defaultEntitySwitch.Case(entityState);

            return (serviceProvider, entity) => {};
        }

        public static IDefaultEntityValueProvider<TEntity> Create()
        {
            return new DefaultEntityProvider<TEntity>();
        }

        private DefaultEntityProvider()
        {
            defaultEntitySwitch = DefaultSwitch.Create<EntityState, Action<IServiceProvider, TEntity>>();
        }

        public IDefaultEntityValueProvider<TEntity> AddDefaults(EntityState entityState, Action<IServiceProvider, TEntity> action)
        {
            defaultEntitySwitch.CaseWhen(entityState, action);
            return this; 
        }

        private readonly ISwitch<EntityState, Action<IServiceProvider, TEntity>> defaultEntitySwitch;
    }
}
