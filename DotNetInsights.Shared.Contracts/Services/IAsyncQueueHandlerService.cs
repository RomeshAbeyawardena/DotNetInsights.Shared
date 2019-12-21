using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface IAsyncQueueHandlerService<TQueueItem> : IQueueHandlerService<TQueueItem, Task>, IAsyncDisposable
    {
        
    }
}