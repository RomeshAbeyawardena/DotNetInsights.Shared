using DotNetInsights.Shared.Domains.Enumerations;
using System.Text;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface IEncryptionService
    {
        byte[] GenerateKey(SymmetricAlgorithmType symmetricAlgorithmType);
        byte[] GenerateIv(SymmetricAlgorithmType symmetricAlgorithmType);
        byte[] GenerateBytes(string key, Encoding encoding = null);
        Task<byte[]> EncryptString(SymmetricAlgorithmType symmetricAlgorithmType, string plainText, byte[] key, byte[] iV);
        Task<string> DecryptBytes(SymmetricAlgorithmType symmetricAlgorithmType,byte[] bytes, byte[] key, byte[] iV);
    }
}
