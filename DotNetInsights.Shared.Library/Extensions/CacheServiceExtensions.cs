using DotNetInsights.Shared.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class CacheServiceExtensions
    {
        public static async Task<IEnumerable<T>> Append<T>(this ICacheService cacheService, string cacheKeyName, T value)
        {
            if(cacheService == null)
                throw new ArgumentNullException(nameof(cacheService));
;
            var cachedValue = await cacheService.Get<IEnumerable<T>>(cacheKeyName).ConfigureAwait(false);

            if(cachedValue == null)
                cachedValue = Array.Empty<T>();

            cachedValue  = cachedValue.Append(value);

            return await cacheService.Set(cacheKeyName, cachedValue).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> Remove<T>(this ICacheService cacheService, string cacheKeyName, T value)
        {
            if(cacheService == null)
                throw new ArgumentNullException(nameof(cacheService));

            var cachedValue = await cacheService.Get<IEnumerable<T>>(cacheKeyName).ConfigureAwait(false);
            
            if(cachedValue == null)
                return Array.Empty<T>();

            cachedValue = cachedValue.Remove(value);

            return await cacheService.Set(cacheKeyName, cachedValue).ConfigureAwait(false);
        }
    }
}
