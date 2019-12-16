namespace DotNetInsights.Shared.Contracts
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T source) where T : class;
        T Deserialize<T>(byte[] valueBytes)  where T : class;
    }
}