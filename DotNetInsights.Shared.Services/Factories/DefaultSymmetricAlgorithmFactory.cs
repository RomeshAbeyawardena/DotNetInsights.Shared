using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using System;
using System.Security.Cryptography;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services.Factories
{
    public class DefaultSymmetricAlgorithmFactory : ISymmetricAlgorithmFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISwitch<SymmetricAlgorithmType, Type> symmetricAlgorithmTypeSwitch;

        public SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricAlgorithmType symmetricAlgorithmType)
        {
           var symmetricAlgorithm = symmetricAlgorithmTypeSwitch.Case(symmetricAlgorithmType);
           return (SymmetricAlgorithm)serviceProvider.GetService(symmetricAlgorithm);
        }

        public DefaultSymmetricAlgorithmFactory(IServiceProvider serviceProvider, ISwitch<SymmetricAlgorithmType, Type> symmetricAlgorithmTypeSwitch)
        {
            this.serviceProvider = serviceProvider;
            this.symmetricAlgorithmTypeSwitch = symmetricAlgorithmTypeSwitch;
        }
    }
}
