using System.Collections.Generic;
using System.Text;

namespace DotNetInsights.Shared.Contracts.Providers
{
    public interface ICryptographicProvider
    {
        bool IsHashValid(byte[] hash, byte[] compareHash);
        byte[] ComputeHash(byte[] raw, string algName);
        byte[] GenerateKey(string uniqueKey, ICryptographicInfo cryptographicInfo, int keyLength);
        IEnumerable<byte[]> ExtractDigestValues(Encoding encoding, string base64value, char separator); 
    }
}
