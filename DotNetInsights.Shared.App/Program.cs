using DotNetInsights.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DotNetInsights.Shared.Services.Extensions;
using DotNetInsights.Shared.Library.Extensions;
using DotNetInsights.Shared.Contracts;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetInsights.Shared.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await  AppHost.CreateBuilder()
                .RegisterServices(serviceRegistration)
                .ConfigureLogging(logging => logging.AddConsole())
                .Build<Startup>()
                .RunAsync(a => a.Begin());
        }

        private static void serviceRegistration(IServiceCollection services)
        {
            services
                .RegisterServiceBroker<ServiceRegistration>(ServiceLifetime.Singleton)
                .AddLogging();
        }

        public class ServiceRegistration : DefaultServiceBroker
        {
            public override IEnumerable<Assembly> GetAssemblies => new [] { DefaultAssembly };
        }
    }
}
