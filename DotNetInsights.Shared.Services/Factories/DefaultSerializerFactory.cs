using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using System;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services.Factories
{
    public sealed class DefaultSerializerFactory : ISerializerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISwitch<SerializerType, Type> _serializerFactory;

        public IBinarySerializer GetSerializer(SerializerType serializer)
        {
            var serializerType = _serializerFactory.Case(serializer);
            
            if(serializerType == null)
                throw new NullReferenceException();

            var service = _serviceProvider.GetService(serializerType);

            return (IBinarySerializer)service;
        }

        public DefaultSerializerFactory(IServiceProvider serviceProvider, ISwitch<SerializerType, Type> serializerFactory)
        {
            _serviceProvider = serviceProvider;
            _serializerFactory = serializerFactory;
        }
    }
}
