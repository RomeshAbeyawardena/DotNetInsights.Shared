using DotNetInsights.Shared.Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using DotNetInsights.Shared.Library.Attributes;
using DotNetInsights.Shared.Contracts.Services;
using System.Threading.Tasks;
using DotNetInsights.Shared.Contracts;
using System.Text;

namespace DotNetInsights.Shared.Services.Providers
{
    public class DefaultDomainEncryptionProvider : IDomainEncryptionProvider
    {
        public async Task<TDest> Decrypt<TSource, TDest>(TSource value, ICryptographicInfo cryptographicInfo)
        {
            var sourceProperties = value.GetType().GetProperties();
            var destProperties = typeof(TDest).GetProperties();
            var encryptionKeyProperties = GetPropertiesWithCustomAttribute<EncryptionKeyAttribute>(sourceProperties);
            var encryptionKeyProperty = encryptionKeyProperties.SingleOrDefault();

            var encryptedProperties = GetPropertiesWithCustomAttribute<EncryptableAttribute>(sourceProperties);

            var destination = Activator.CreateInstance<TDest>();
            var encryptionKey = encryptionKeyProperty.GetValue(value) as byte[]; 
            
            foreach(var property in sourceProperties){
                var destinationProperty = GetProperty(destProperties, property.Name);
                var rawValue = property.GetValue(value);

                if(rawValue == null)
                    continue;

                var encryptedValue = rawValue as IEnumerable<byte>;

                if(encryptedProperties.Contains(property))
                    destinationProperty?.SetValue(destination, await _encryptionService
                        .DecryptBytes(cryptographicInfo.SymmetricAlgorithmType, encryptedValue.ToArray(), encryptionKey, cryptographicInfo.InitialVector.ToArray())
                        .ConfigureAwait(false));
                else
                    destinationProperty?.SetValue(destination, rawValue);
            }

            return destination;
        }

        public async Task<TDest> Encrypt<TSource, TDest>(TSource value, ICryptographicInfo cryptographicInfo)
        {
            var sourceProperties = value.GetType().GetProperties();
            var destProperties = typeof(TDest).GetProperties();
            var encryptionKeyProperties = GetPropertiesWithCustomAttribute<EncryptionKeyAttribute>(destProperties);
            
            var encryptionCandidateProperties = GetPropertiesWithCustomAttribute<EncryptionKeyCandidateAttribute>(sourceProperties);

            var encryptionKey = GenerateUniqueKeyFromEncryptionKeyCandidates(value, encryptionCandidateProperties);

            var encryptionKeyProperty = encryptionKeyProperties.SingleOrDefault();
            var destination = Activator.CreateInstance<TDest>();
            var generatedKey = _cryptographicProvider.GenerateKey(encryptionKey, cryptographicInfo, 32);

            encryptionKeyProperty.SetValue(destination, generatedKey);

            foreach (var sourceProperty in sourceProperties)
            {
                var propertyValue = sourceProperty.GetValue(value);
                var destinationProperty = GetProperty(destProperties, sourceProperty.Name);
                if(sourceProperty.GetCustomAttribute<EncryptableAttribute>() != null && propertyValue != null)
                    destinationProperty?.SetValue(destination, await _encryptionService
                        .EncryptString(cryptographicInfo.SymmetricAlgorithmType, propertyValue.ToString(), generatedKey, cryptographicInfo.InitialVector.ToArray())
                        .ConfigureAwait(false) );
                else
                    destinationProperty?.SetValue(destination, propertyValue);
            }

            return destination;
        }

        private string GenerateUniqueKeyFromEncryptionKeyCandidates<TSource>(TSource value, IEnumerable<PropertyInfo> encryptionCandidateProperties)
        {
            var uniqueKeyStringBuilder = new StringBuilder();

            foreach(var encryptedCandidateProperty in encryptionCandidateProperties)
            {
                var encryptionKeyCandidate = encryptedCandidateProperty.GetCustomAttribute<EncryptionKeyCandidateAttribute>();
                uniqueKeyStringBuilder.AppendFormat(encryptionKeyCandidate.FormatProvider, encryptionKeyCandidate.Format, encryptedCandidateProperty.GetValue(value));
            }

            return uniqueKeyStringBuilder.ToString();
        }

        private PropertyInfo GetProperty(IEnumerable<PropertyInfo> properties, string propertyName)
        {
            return properties.SingleOrDefault(prop => prop.Name.Equals(propertyName, StringComparison.InvariantCulture));
        }

        private IEnumerable<PropertyInfo> GetPropertiesWithCustomAttribute<TCustomAttribute>(IEnumerable<PropertyInfo> properties)
            where TCustomAttribute : Attribute
        {
            return properties.Where(property => property.GetCustomAttribute<TCustomAttribute>() != null);
        }

        public async Task<IEnumerable<TDest>> Decrypt<TSource, TDest>(IEnumerable<TSource> values, ICryptographicInfo cryptographicInfo)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            if(cryptographicInfo == null)
                throw new ArgumentNullException(nameof(cryptographicInfo));

            var decryptedItems = new List<TDest>();

            foreach (var value in values)
            {
                decryptedItems.Add(
                    await Decrypt<TSource, TDest>(value, cryptographicInfo)
                        .ConfigureAwait(false));
            }

            return decryptedItems.ToArray();
        }

        public async Task<IEnumerable<TDest>> Encrypt<TSource, TDest>(IEnumerable<TSource> values, ICryptographicInfo cryptographicInfo)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            if(cryptographicInfo == null)
                throw new ArgumentNullException(nameof(cryptographicInfo));

            var decryptedItems = new List<TDest>();

            foreach (var value in values)
            {
                decryptedItems.Add(
                    await Encrypt<TSource, TDest>(value, cryptographicInfo)
                        .ConfigureAwait(false));
            }

            return decryptedItems.ToArray();
        }

        public DefaultDomainEncryptionProvider(ICryptographicProvider cryptographicProvider, IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
            _cryptographicProvider = cryptographicProvider;
        }

        private readonly IEncryptionService _encryptionService;
        private readonly ICryptographicProvider _cryptographicProvider;
    }
}
