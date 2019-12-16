using System.Collections.Generic;
using System.Text;

namespace DotNetInsights.Shared.Contracts.Providers
{
    public interface IEncodingProvider
    {
        IEnumerable<Encoding> Encodings { get; }
        Encoding GetEncoding(IEnumerable<Encoding> encoding, string encodingName);
    }
}
