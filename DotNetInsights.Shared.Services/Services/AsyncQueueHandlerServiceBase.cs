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
            await DisposeAsync(true);
        }
        
        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return new ValueTask(DisposeAsync(true));
            
        }

        protected virtual async Task DisposeAsync(bool gc)
        {
            _asyncQueueServiceTimer.Dispose();
            await FlushQueue();
        }

        protected async Task FlushQueue()
        {
            while(!_asyncQueueServiceQueue.IsEmpty)
                if(_asyncQueueServiceQueue.TryDequeue(out var queueItem))
                    await ProcessQueueItem(queueItem);
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
                _logger.LogDebug("Queue empty: waiting on Queue...", _options.PollingInterval);
                await QueueHasItems();
            }

            _logger.LogInformation("Processing queue items...");
            await FlushQueue();

            _asyncQueueServiceTimer.Change(_asyncQueueServiceQueue.IsEmpty 
                ? _options.PollingInterval 
                : _options.ProcessingInterval, Timeout.Infinite);
        }

        private async Task QueueHasItems()
        {
            while(_asyncQueueServiceQueue.IsEmpty)
                await Task.Delay(1000);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool gc)
        {
            _asyncQueueServiceTimer.Dispose();
        }

        private readonly ILogger<IAsyncQueueHandlerService<TQueueItem>> _logger;
        private readonly TQueueServiceOptions _options;
        private readonly ConcurrentQueue<TQueueItem> _asyncQueueServiceQueue;
        private readonly Timer _asyncQueueServiceTimer;
    }
}