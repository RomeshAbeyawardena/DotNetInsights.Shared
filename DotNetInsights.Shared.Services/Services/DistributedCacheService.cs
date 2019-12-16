using Microsoft.Extensions.Caching.Distributed;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Services;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public class DistributedMemoryCacheService : ICacheService
    {
        private readonly IDistributedCache distributedCache;
        private readonly IMessagePackBinarySerializer messagePackBinarySerializer;
        private readonly DistributedCacheEntryOptions distributedCacheEntryOptions;

        public async Task<T> Get<T>(string cacheKeyName) where T : class
        {
            var cachedResult = await distributedCache.GetAsync(cacheKeyName).ConfigureAwait(false);

            if (cachedResult == null || cachedResult.Length == 0)
                return default;

            return messagePackBinarySerializer.Deserialize<T>(cachedResult);
        }

        public async Task<T> Set<T>(string cacheKeyName, T value) where T : class
        {
            if (value == null)
                return value;

            var data = messagePackBinarySerializer.Serialize(value);
            
            await distributedCache.SetAsync(cacheKeyName, data).ConfigureAwait(false);

            return value;
        }

        public async Task RemoveAsync(string key)
        {
            await distributedCache.RemoveAsync(key).ConfigureAwait(false);
        }

        public DistributedMemoryCacheService(IDistributedCache distributedCache,
            IMessagePackBinarySerializer messagePackBinarySerializer)
        {
            this.distributedCache = distributedCache;
            this.messagePackBinarySerializer = messagePackBinarySerializer;
            distributedCacheEntryOptions = new DistributedCacheEntryOptions();
            
            
        }
    }
}
