using DotNetInsights.Shared.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNetInsights.Shared.Services
{
    public abstract class DefinedServiceScopeBase : IDefinedServiceScope
    {
        public IServiceScope ServiceScope { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool gc)
        {
            ServiceScope.Dispose();
        }

        protected DefinedServiceScopeBase(IServiceProvider serviceProvider)
        {
            ServiceScope = serviceProvider.CreateScope()
        }
    }
}