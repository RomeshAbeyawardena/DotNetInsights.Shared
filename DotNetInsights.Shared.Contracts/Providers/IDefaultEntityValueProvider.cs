using Microsoft.EntityFrameworkCore;
using System;

namespace DotNetInsights.Shared.Contracts.Providers
{
    public interface IDefaultEntityValueProvider<TEntity>
        where TEntity : class
    {
        IDefaultEntityValueProvider<TEntity> AddDefaults(EntityState entityState, Action<IServiceProvider, TEntity> action);
        Action<IServiceProvider, TEntity> GetDefaultAssignAction(EntityState entityState);
    }
}
