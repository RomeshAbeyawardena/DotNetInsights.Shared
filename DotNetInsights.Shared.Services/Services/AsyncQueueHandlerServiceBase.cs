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
            await FlushQueue();
            _asyncQueueServiceTimer.Dispose();
            
        }

        protected async Task FlushQueue()
        {
            while(!_asyncQueueServiceQueue.IsEmpty)
                if(_asyncQueueServiceQueue.TryDequeue(out var queueItem))
                    await ProcessQueueItem(queueItem);
        }


        public AsyncQueueHandlerServiceBase(ILogger<IAsyncQueueHandlerService<TQueueItem>> logger, ConcurrentQueue<TQueueItem> asyncQueueServiceQueue,
            TQueueServiceOptions asyncQueueServiceOptions, bool logQueueProcess)
        {
            _logger = logger;
            _options = asyncQueueServiceOptions;
            _asyncQueueServiceQueue = asyncQueueServiceQueue;
            _asyncQueueServiceTimer = new Timer(async(state) => 
                await ProcessQueue(state), null,
                _options.PollingInterval, Timeout.Infinite);
            _logQueueProcess = logQueueProcess;
        }

        private async Task ProcessQueue(object state)
        {
            Log(logger => logger.LogInformation("Polling queue..."));
            if(_asyncQueueServiceQueue.IsEmpty){

                Log(logger => logger.LogDebug("Queue empty: waiting on Queue...", _options.PollingInterval));
                await QueueHasItems();
            }

            Log(logger => logger.LogInformation("Processing queue items..."));
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

        private void Log(Action<ILogger<IAsyncQueueHandlerService<TQueueItem>>> loggerAction)
        {
            if(_logQueueProcess)
                loggerAction(_logger);
        }

        private readonly ILogger<IAsyncQueueHandlerService<TQueueItem>> _logger;
        private readonly TQueueServiceOptions _options;
        private readonly ConcurrentQueue<TQueueItem> _asyncQueueServiceQueue;
        private readonly Timer _asyncQueueServiceTimer;
        private readonly bool _logQueueProcess;
    }
}