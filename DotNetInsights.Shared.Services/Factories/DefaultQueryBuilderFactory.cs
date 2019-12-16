using Microsoft.Extensions.DependencyInjection;
using DotNetInsights.Shared.Contracts.Builders;
using DotNetInsights.Shared.Contracts.Factories;
using System;
using System.Linq.Expressions;

namespace DotNetInsights.Shared.Services.Factories
{
    public sealed class DefaultQueryBuilderFactory : IQueryBuilderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public Expression<Func<TEntity, bool>> GetExpression<TEntity, TEnum>(TEnum? enumeration)
            where TEntity : class
            where TEnum : struct
        {
            var queryBuilder = serviceProvider.GetRequiredService<IQueryBuilder<TEntity, TEnum>>();
            if(queryBuilder == null)
                throw new NotImplementedException($"IQueryBuilder<{nameof(TEntity)},{nameof(TEnum)}> not implemented. Ensure all instances are registered in ServiceRegistration");

            return queryBuilder.GetExpression(enumeration);
        }

        public DefaultQueryBuilderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
