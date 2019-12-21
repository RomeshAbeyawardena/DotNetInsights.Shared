using DotNetInsights.Shared.Contracts.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DotNetInsights.Shared.Services
{
    public abstract class QueueServiceBase<TQueueItem> : IQueueService<TQueueItem>
    {
        public ConcurrentQueue<TQueueItem> Queue { get; }

        public void Enqueue(TQueueItem queueItem)
        {
            Queue.Enqueue(queueItem);
        }

        protected QueueServiceBase(ConcurrentQueue<TQueueItem> concurrentQueue)
        {
            Queue = concurrentQueue;
        }
    }
}
