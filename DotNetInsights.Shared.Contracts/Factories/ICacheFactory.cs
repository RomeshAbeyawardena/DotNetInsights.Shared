using DotNetInsights.Shared.Contracts.Services;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Contracts.Factories
{
    public interface ICacheFactory
    {
        ICacheService GetCacheService(CacheType cacheServiceType);
    }
}
