using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DotNetInsights.Shared.Contracts.Builders
{
    public interface IAppHostBuilder
    {
        IAppHost<TStartup> Build<TStartup>(IServiceCollection services = null, Action<IServiceProvider> serviceProvider = null) where TStartup : class;
        IAppHostBuilder RegisterServices(Action<IServiceCollection> services);
        IAppHost Build(IServiceCollection services = null);
        IAppHostBuilder UseStartup<TStartup>();
        IAppHostBuilder ConfigureLogging(Action<ILoggingBuilder> buildLogger);
        IAppHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configuration);
    }
}
