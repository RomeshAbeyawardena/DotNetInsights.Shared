using DotNetInsights.Shared.Contracts.Services;
using DotNetInsights.Shared.Library;
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
            var loggingService = ServiceScope.ServiceProvider
                .GetRequiredService<ILoggingService>();

            await loggingService.LogEntry(queueItem);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await DisposeAsync();
        }

        public SqlLoggingHostedService(IServiceProvider serviceProvider, ILogger<SqlLoggingHostedService> logger,
            ConcurrentQueue<SqlLoggerQueueItem> queue,
            SqlLoggerOptions sqlLoggerOptions)
            : base(logger, serviceProvider, queue, sqlLoggerOptions, false) //don't open this big can of worms by changing false to true!!
        {

        }
    }
}