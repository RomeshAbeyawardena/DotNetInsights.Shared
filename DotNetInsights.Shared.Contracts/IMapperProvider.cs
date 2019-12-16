using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts
{
    public interface IMapperProvider
    {
        TDestination Map<TSource, TDestination>(TSource source);
        IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
    }
}
