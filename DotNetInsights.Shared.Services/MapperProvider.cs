using AutoMapper;
using DotNetInsights.Shared.Contracts;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services
{
    public sealed class MapperProvider : IMapperProvider
    {
        private readonly IMapper mapper;

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return mapper.Map<TSource, TDestination>(source);
        }

        public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source);
        }

        public MapperProvider(IMapper mapper)
        {
            this.mapper = mapper;
        }
    }
}
