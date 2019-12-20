using System;

namespace DotNetInsights.Shared.Contracts
{
    public interface IQueueHandlerService<TQueueItem, TResult> : IDisposable
    {
        TResult ProcessQueueItem(TQueueItem queueItem);
    }
}