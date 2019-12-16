using Microsoft.EntityFrameworkCore;

namespace DotNetInsights.Shared.Contracts.Factories
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TDbContext, TEntity>() 
            where TDbContext: DbContext
            where TEntity: class;
    }
}
