using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Library;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    internal class DefaultAppHost : IAppHost
    {
        private readonly Type startupType;
        protected readonly IServiceProvider serviceProvider;
        protected readonly ILogger logger;

        protected object StartupService => serviceProvider.GetService(startupType);

        public DefaultAppHost(Type startupType, IServiceProvider serviceProvider)
        {
            this.startupType = startupType;
            this.serviceProvider = serviceProvider;
            
        }

        public object Run(string methodName)
        {
            var method = startupType.GetMethod(methodName);
            return method.Invoke(StartupService, Array.Empty<object>());
        }

        public async Task RunAsync(string methodName)
        {
            var runMethod = Run(methodName) as Task;
            await runMethod.ConfigureAwait(false);
        }

        public async Task<T> RunAsync<T>(string methodName)
        {
            var runMethod = Run(methodName) as Task<T>;
            return await runMethod.ConfigureAwait(false);
        }
    }

    internal class DefaultAppHost<TStartup> : DefaultAppHost, IAppHost<TStartup>
    {
        protected new TStartup StartupService => (TStartup)base.StartupService;
        protected new ILogger<TStartup> logger;
        public DefaultAppHost(IServiceProvider serviceProvider) 
            : base(typeof(TStartup), serviceProvider)
        {
            this.logger = serviceProvider.GetRequiredService<ILogger<TStartup>>();
        }

        public object Run(Func<TStartup, object> getMember)
        {
            //, ex => logger.LogError(ex, "An error occurred"), catchAll: true
            return ExceptionHandler
                .Try<TStartup, object>((a) => getMember(a))
                .Catch(exception => logger.LogError(exception, "An error has occurred {0}", exception.Message), typeof(Exception))
                .Instance
                .Invoke(StartupService);
        }

        public async Task RunAsync(Func<TStartup, Task<object>> getMemberTask)
        {
            await RunAsync(getMemberTask);
            //await getMemberTask.TryAsync(StartupService, ex => logger.LogError(ex, "An error occurred"), catchAll: true).ConfigureAwait(false);
        }

        public async Task<T> RunAsync<T>(Func<TStartup, Task<T>> getMemberTask)
        {
             return await ExceptionHandler.TryAsync<TStartup, T>(async(startup) => await getMemberTask(startup))
               .Invoke(StartupService);
            //return await getMemberTask.TryAsync(StartupService, ex => logger.LogError(ex, "An error occurred"), catchAll: true).ConfigureAwait(false);
        }
    }
}
