using DotNetInsights.Shared.Domains.Enumerations;
using System;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts
{
    public interface ICryptographicInfo
    {
        SymmetricAlgorithmType SymmetricAlgorithmType { get; }
        [Obsolete("Key is no longer used by Cryptographic providers")]
        IEnumerable<byte> Key { get; }

        string Password { get; }
        IEnumerable<byte> Salt { get; }
        IEnumerable<byte> InitialVector { get; }
        int Iterations { get; }
    }
}