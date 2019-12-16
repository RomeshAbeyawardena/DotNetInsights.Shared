using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Providers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace DotNetInsights.Shared.Services
{
    public class DefaultRepository<TDbContext, TEntity> : IRepository<TEntity>
        where TDbContext : DbContext
            where TEntity : class
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TDbContext dbContext;

        public DefaultRepository(IServiceProvider serviceProvider, TDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
        }

        public DbSet<TEntity> DbSet => dbContext.Set<TEntity>();

        public async Task<TEntity> FindAsync(object key)
        {
            return await DbSet.FindAsync(key).ConfigureAwait(false);
        }

        public async Task<TEntity> FindAsync(params object[] key)
        {
            return await DbSet.FindAsync(key).ConfigureAwait(false);
        }

        public async Task<int> Remove(TEntity entity, bool saveChanges = true)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            dbContext.Remove(entity);
            return await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> RemoveAsync(bool saveChanges = true, params object[] keys)
        {
            var entity = await FindAsync(keys).ConfigureAwait(false);
            return await Remove(entity).ConfigureAwait(false);
        }

        private EntityState GetEntityState(TEntity entity)
        {
            var model = dbContext.Model.GetEntityTypes().SingleOrDefault(entityType => entityType.ClrType == typeof(TEntity));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            foreach (var key in model.GetKeys())
            {
                var keyPropertyInfo = key.Properties.SingleOrDefault().PropertyInfo;

                var nullObject = keyPropertyInfo.PropertyType.IsValueType 
                    ? Activator.CreateInstance(keyPropertyInfo.PropertyType) 
                    : null;
                
                var keyValue = keyPropertyInfo.GetValue(entity);
                if(keyValue.Equals(nullObject))
                    return EntityState.Added;
                else
                    return EntityState.Modified;
            }
            
            return EntityState.Unchanged;
        }

        public async Task<TEntity> SaveChangesAsync(TEntity entity, bool saveChanges = true)
        {
            var entityState = GetEntityState(entity);

            var defaultEntityProvider = serviceProvider.GetRequiredService<IDefaultEntityValueProvider<TEntity>>();

            defaultEntityProvider.GetDefaultAssignAction(entityState)?.Invoke(serviceProvider, entity);

            if(entityState == EntityState.Added)
                DbSet.Add(entity);
            else if(entityState == EntityState.Modified)
                DbSet.Update(entity);

            if(saveChanges)
                await SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> whereExpression = null, bool detachEntities = true)
        {
            var query = detachEntities 
                ? DbSet.AsNoTracking() 
                : DbSet;
            
            if(whereExpression == null)
                return query;
            
            return query.Where(whereExpression);
        }

        public async Task BeginTransaction(TransactionScopeOption transactionScopeOption, Func<TransactionScope,Task> transactionScope)
        {
            using (var scope = new TransactionScope(transactionScopeOption))
                await transactionScope(scope).ConfigureAwait(false);
        }

        public async Task<T> BeginTransaction<T>(TransactionScopeOption transactionScopeOption, Func<TransactionScope, Task<T>> transactionScope)
        {
            using (var scope = new TransactionScope(transactionScopeOption))
                return await transactionScope(scope).ConfigureAwait(false);
        }

    }
}
