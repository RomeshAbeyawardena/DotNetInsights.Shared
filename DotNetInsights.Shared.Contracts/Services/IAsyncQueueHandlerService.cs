using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface IAsyncQueueHandlerService<TQueueItem> : IAsyncDisposable
    {
        Task ProcessQueueItem(TQueueItem queueItem);
    }
}