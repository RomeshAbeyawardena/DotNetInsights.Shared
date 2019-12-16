using System;
using System.Collections.Generic;
using System.Reflection;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using DotNetInsights.Shared.Library;
using DotNetInsights.Shared.Library.Extensions;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services
{
    public class DefaultCloner<T> : ICloner<T> where T : class
    {
        private readonly DefaultCloneOptions _defaultCloneOptions;
        private readonly ISerializerFactory serializerFactory;

        public DefaultCloner(IOptions<DefaultCloneOptions> cloneOptions, ISerializerFactory serializerFactory)
        {
            _defaultCloneOptions = cloneOptions.Setup();
            this.serializerFactory = serializerFactory;
        }

        public T Clone(T source, CloneType cloneType)
        {
            switch (cloneType)
            {
                case CloneType.Deep:
                    return DeepClone(source);
                case CloneType.Shallow:
                    return ShallowClone(source);
                default:
                    if(_defaultCloneOptions.DefaultCloneType == CloneType.Deep 
                       || _defaultCloneOptions.DefaultCloneType == CloneType.Shallow)
                        return Clone(source, _defaultCloneOptions.DefaultCloneType);
                    throw new NotSupportedException();
            }
        }

        public T ShallowClone(T source, IEnumerable<PropertyInfo> properties = null)
        {
            var sourceType = source.GetType();
            
            var newInstance = Activator.CreateInstance<T>();

            sourceType.ForProperties(source, newInstance, (propertyInfo, src, instance) =>
            {
                var value = propertyInfo.GetValue(src);

                if(value != null)
                    propertyInfo.SetValue(instance, value);
            });

            return newInstance;
        }

        public T DeepClone(T source)
        {
            var binarySerializer = serializerFactory.GetSerializer(_defaultCloneOptions.SerializerType);
            var serialized = binarySerializer.Serialize(source);
            return binarySerializer.Deserialize<T>(serialized);
        }
    }
}
