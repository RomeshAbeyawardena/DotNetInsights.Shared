using Microsoft.EntityFrameworkCore;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using DotNetInsights.Shared.Library.Extensions;
using System;

namespace DotNetInsights.Shared.Services.Factories
{
    public class DefaultRepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider serviceProvider;

        public IRepository<TEntity> GetRepository<TDbContext, TEntity>()
            where TDbContext : DbContext
            where TEntity : class
        {
            return serviceProvider.Resolve<DefaultRepository<TDbContext, TEntity>>();
        }

        public DefaultRepositoryFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
