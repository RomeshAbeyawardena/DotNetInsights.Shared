using DotNetInsights.Shared.Contracts.Builders;
using DotNetInsights.Shared.Services.Builders;

namespace DotNetInsights.Shared.Services
{
    public static class AppHost
    {
        public static IAppHostBuilder CreateBuilder()
        {
            return new DefaultAppHostBuilder();
        }
    }
}
