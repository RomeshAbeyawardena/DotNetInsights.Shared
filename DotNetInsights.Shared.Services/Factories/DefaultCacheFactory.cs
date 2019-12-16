using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using DotNetInsights.Shared.Contracts.Services;
using DotNetInsights.Shared.Library.Extensions;
using System;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services.Factories
{
    public class DefaultCacheFactory : ICacheFactory
    {
        public ICacheService GetCacheService(CacheType cacheServiceType)
        {
            return (ICacheService)serviceProvider.Resolve(_cacheServiceSwitch.Case(cacheServiceType));
        }

        ICacheService ICacheFactory.GetCacheService(CacheType cacheServiceType)
        {
            return GetCacheService(cacheServiceType);
        }

        public DefaultCacheFactory(IServiceProvider serviceProvider, ISwitch<CacheType, Type> cacheServiceSwitch)
        {
            this.serviceProvider = serviceProvider;
            _cacheServiceSwitch = cacheServiceSwitch;
        }

        private readonly ISwitch<CacheType, Type> _cacheServiceSwitch;
        private readonly IServiceProvider serviceProvider;
    }
}
