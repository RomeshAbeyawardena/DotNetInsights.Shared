using DotNetInsights.Shared.Contracts.Services;
using DotNetInsights.Shared.Library.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.HostedServices
{
    public class SqlLoggingHostedService : AsyncQueueHandlerServiceBase<SqlLoggerQueueItem, SqlLoggerOptions>, IHostedService
    {
        public override async Task ProcessQueueItem(SqlLoggerQueueItem queueItem)
        {
            using var serviceScope = _serviceProvider.CreateScope();

            var loggingService = serviceScope.ServiceProvider.GetRequiredService<ILoggingService>();
            
            await loggingService.LogEntry(queueItem);
        }

        

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public SqlLoggingHostedService(IServiceProvider serviceProvider, ILogger<SqlLoggingHostedService> logger, 
            ConcurrentQueue<SqlLoggerQueueItem> queue, 
            SqlLoggerOptions sqlLoggerOptions)
            : base (logger, queue, sqlLoggerOptions)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;
    }
}