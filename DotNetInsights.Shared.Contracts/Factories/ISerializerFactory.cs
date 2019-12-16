using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Contracts.Factories
{
    public interface ISerializerFactory
    {
        IBinarySerializer GetSerializer(SerializerType serializer);
    }
}
