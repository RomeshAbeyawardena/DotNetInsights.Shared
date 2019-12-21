namespace DotNetInsights.Shared.Contracts.Services
{
    public interface IQueueService<TQueueItem>
    {
        public void Enqueue(TQueueItem queueItem);
    }
}