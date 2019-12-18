using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public abstract class AsyncQueueHandlerServiceBase<TQueueItem, TQueueServiceOptions> : IAsyncQueueHandlerService<TQueueItem>
        where TQueueServiceOptions : IAsyncQueueServiceOptions
    {
        public abstract Task ProcessQueueItem(TQueueItem queueItem);
        
        public async Task DisposeAsync()
        {
            await Dispose(true);
        }
        
        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return new ValueTask();
        }

        protected virtual async Task Dispose(bool gc)
        {
            _asyncQueueServiceTimer.Dispose();
            await FlushQueue();
        }

        public AsyncQueueHandlerServiceBase(ILogger<IAsyncQueueHandlerService<TQueueItem>> logger, ConcurrentQueue<TQueueItem> asyncQueueServiceQueue,
            TQueueServiceOptions asyncQueueServiceOptions)
        {
            _logger = logger;
            _options = asyncQueueServiceOptions;
            _asyncQueueServiceQueue = asyncQueueServiceQueue;
            _asyncQueueServiceTimer = new Timer(async(state) => 
                await ProcessQueue(state), null,
                _options.PollingInterval, Timeout.Infinite);
        }

        private async Task ProcessQueue(object state)
        {
            _logger.LogInformation("Polling queue...");
            if(_asyncQueueServiceQueue.IsEmpty){
                _logger.LogDebug("Queue empty - polling mode");
                _asyncQueueServiceTimer.Change(_options.PollingInterval, Timeout.Infinite);
                return;
            }
            
            await FlushQueue();

            _logger.LogInformation("Processing queue item - processing mode");

            _asyncQueueServiceTimer.Change(_asyncQueueServiceQueue.IsEmpty 
                ? _options.PollingInterval 
                : _options.ProcessingInterval, Timeout.Infinite);
        }

        protected async Task FlushQueue()
        {
            while(!_asyncQueueServiceQueue.IsEmpty)
                if(_asyncQueueServiceQueue.TryDequeue(out var queueItem))
                    await ProcessQueueItem(queueItem);
        }


        private readonly ILogger<IAsyncQueueHandlerService<TQueueItem>> _logger;
        private readonly TQueueServiceOptions _options;
        private readonly ConcurrentQueue<TQueueItem> _asyncQueueServiceQueue;
        private readonly Timer _asyncQueueServiceTimer;
    }
}