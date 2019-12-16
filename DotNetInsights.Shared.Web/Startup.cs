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

namespace DotNetInsights.Shared.WebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RegisterServiceBroker<AppQueueServiceBroker>(ServiceLifetime.Scoped)
                .AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddScoped<IMyScopedService, MyScopedService>()
                .ConfigureNotificationsHostedServiceOptions(options => {
                    options.PollingInterval = 60000;
                    options.ProcessingInterval = 60;
                })
                .AddHostedService<NotificationsHostedService>()
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
        public void Execute()
        {
            Console.WriteLine("Subscriber OnChange called!");
        }
    }

    public class AppQueueServiceBroker : DefaultServiceBroker
    {
        public override IEnumerable<Assembly> GetAssemblies => new [] { 
            DefaultAssembly, 
            Assembly.GetAssembly(typeof(AppQueueServiceBroker)) };
    }
}
