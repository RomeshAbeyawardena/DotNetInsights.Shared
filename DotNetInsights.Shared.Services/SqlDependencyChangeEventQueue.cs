using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Library;
using System.Collections.Concurrent;

namespace DotNetInsights.Shared.Services
{
    public sealed class SqlDependencyChangeEventQueue : QueueServiceBase<SqlDependencyChangeEventQueueItem>
    {
        public SqlDependencyChangeEventQueue(ConcurrentQueue<SqlDependencyChangeEventQueueItem> concurrentQueue) 
            : base(concurrentQueue)
        {
        }
    }
}