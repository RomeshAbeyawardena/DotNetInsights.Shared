using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface IAsyncQueueService<TQueueItem> : IAsyncDisposable
    {
        Task ProcessQueueItem(TQueueItem queueItem);
    }
}