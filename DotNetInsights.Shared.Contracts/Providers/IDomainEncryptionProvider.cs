using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Providers
{
    public interface IDomainEncryptionProvider
    {
        Task<IEnumerable<TDest>> Decrypt<TSource, TDest>(IEnumerable<TSource> values, ICryptographicInfo cryptographicInfo);
        Task<IEnumerable<TDest>> Encrypt<TSource, TDest>(IEnumerable<TSource> values, ICryptographicInfo cryptographicInfo);
        Task<TDest> Decrypt<TSource, TDest>(TSource value, ICryptographicInfo cryptographicInfo);
        Task<TDest> Encrypt<TSource, TDest>(TSource value, ICryptographicInfo cryptographicInfo);
    }
}