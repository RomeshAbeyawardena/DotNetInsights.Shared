using DotNetInsights.Shared.Contracts.Factories;
using DotNetInsights.Shared.Contracts.Providers;
using DotNetInsights.Shared.Contracts.Services;
using System;
using System.Threading.Tasks;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services.Providers
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private readonly ICacheFactory _cacheFactory;

        public T GetOrDefault<T>(string cacheName, Func<T> value, CacheType cacheType = CacheType.DistributedCache)
            where T: class
        {
            return GetOrDefaultAsync(cacheName, async() => await Task.FromResult(value()).ConfigureAwait(false), cacheType).Result;
        }

        public async Task<T> GetOrDefaultAsync<T>(string cacheName, Func<Task<T>> value, CacheType cacheType = CacheType.DistributedCache)
            where T: class
        {
            var cacheService =  GetCacheService(cacheType);
            var result = await cacheService.Get<T>(cacheName).ConfigureAwait(false);

            if(result != null)
                return result;

            result = await value().ConfigureAwait(false);

            return await cacheService.Set(cacheName, result).ConfigureAwait(false);
        }

        public ICacheService GetCacheService(CacheType cacheType) => _cacheFactory.GetCacheService(cacheType);

        public DefaultCacheProvider(ICacheFactory cacheFactory)
        {
            _cacheFactory = cacheFactory;
        }
    }
}
