using Microsoft.Extensions.DependencyInjection;

namespace DotNetInsights.Shared.Contracts
{
    public interface IServiceRegistration
    {
        void RegisterServices(IServiceCollection services);
    }
}
