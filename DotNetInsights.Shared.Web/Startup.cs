using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetInsights.Shared.Services;
using DotNetInsights.Shared.Library.Extensions;
using System.Reflection;
using AutoMapper;
using DotNetInsights.Shared.Services.Middleware;
using System.Collections.Generic;
using DotNetInsights.Shared.Services.HostedServices;
using DotNetInsights.Shared.Services.Extensions;
using Microsoft.Extensions.Logging;
using DotNetInsights.Shared.Services.Providers;

namespace DotNetInsights.Shared.WebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(configure => configure.AddProvider<SqlLoggerProvider>());

            services
                .RegisterServiceBroker<AppQueueServiceBroker>(ServiceLifetime.Scoped)
                .AddSingleton<ApplicationSettings, ApplicationSettings>()
                .AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddScoped<IMyScopedService, MyScopedService>()
                .ConfigureHostedServiceOptions(options => {
                    options.ConfigureNotifications(notificationOptions => { 
                        notificationOptions.PollingInterval = 30000;
                        notificationOptions.ProcessingInterval = 30; }
                    );
                    options.ConfigureSqlDependency(sqlDependencyOptions =>
                    {
                        sqlDependencyOptions.ConfigureConnectionString = serviceProvider =>
                        {
                            var applicationSettings = serviceProvider.GetRequiredService<ApplicationSettings>();
                            return applicationSettings.ConnectionString;
                        };
                        sqlDependencyOptions.PollingInterval = 30000;
                        sqlDependencyOptions.ProcessingInterval = 30;
                    })
                    .ConfigureSqlLoggerOptions(sqlLoggerOptions => {
                        sqlLoggerOptions.GetConnectionString = serviceProvider => {
                            var applicationSettings = serviceProvider.GetRequiredService<ApplicationSettings>();
                            return applicationSettings.ConnectionString;
                        };
                        sqlLoggerOptions.GetTableSchema = serviceProvider => "dbo";
                        sqlLoggerOptions.GetTableName = serviceProvider => "LogEntry";
                        sqlLoggerOptions.GetLogOptionsTableName = serviceProvider => "LogOptions";
                    });
                })
                .AddHostedService<NotificationsHostedService>()
                .AddHostedService<SqlDependencyHostedService>()
                .AddHostedService<SqlLoggingHostedService>()
                .AddMvc(options => options.Filters.Add<HandleModelStateErrorFilter>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            
            app.UseRouting();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllers();
            });
        }
    }

    public interface IMyScopedService
    {
        void Execute();
    }


    public class MyScopedService : IMyScopedService
    {
        private readonly ILogger<IMyScopedService> _logger;

        public void Execute()
        {
            _logger.LogInformation("Subscriber OnChange called!");
        }

        public MyScopedService(ILogger<IMyScopedService> logger)
        {
            _logger = logger;
        }
    }

    public class AppQueueServiceBroker : DefaultServiceBroker
    {
        public override IEnumerable<Assembly> GetAssemblies => new [] { 
            DefaultAssembly, 
            Assembly.GetAssembly(typeof(AppQueueServiceBroker)) };
    }
}
