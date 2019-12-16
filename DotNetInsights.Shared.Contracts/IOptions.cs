namespace DotNetInsights.Shared.Contracts
{
    public interface IOptions<TOptions> where TOptions : class
    {
        TOptions Setup();
    }
}