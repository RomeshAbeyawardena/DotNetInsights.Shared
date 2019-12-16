using System.IO;

namespace DotNetInsights.Shared.Contracts
{
    public interface IMemoryStreamManager
    {
        MemoryStream GetStream(bool useRecyclableMemoryStreamManager = true, byte[] buffer = null);
        MemoryStream GetStream(byte[] buffer, bool useRecyclableMemoryStreamManager = true);
    }
}
