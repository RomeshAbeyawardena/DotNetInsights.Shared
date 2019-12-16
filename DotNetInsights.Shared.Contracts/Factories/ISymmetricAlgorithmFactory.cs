using DotNetInsights.Shared.Domains.Enumerations;
using System.Security.Cryptography;

namespace DotNetInsights.Shared.Contracts.Factories
{
    public interface ISymmetricAlgorithmFactory
    {
        SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricAlgorithmType symmetricAlgorithmType);
    }
}
