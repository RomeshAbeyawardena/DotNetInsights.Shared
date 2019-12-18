namespace DotNetInsights.Shared.Contracts
{
    public interface IAsyncQueueServiceOptions
    {
        int PollingInterval { get; set; }
        int ProcessingInterval { get; set; }
    }
}