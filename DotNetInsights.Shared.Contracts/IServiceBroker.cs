using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetInsights.Shared.Contracts
{
    public interface IServiceBroker
    {
        IEnumerable<Assembly> GetAssemblies { get; }
        void RegisterServiceAssemblies(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies);
    }
}
