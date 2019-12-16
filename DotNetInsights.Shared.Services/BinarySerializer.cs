using System;
using System.Runtime.Serialization.Formatters.Binary;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Services
{
    public class BinarySerializer : IBinarySerializer
    {
        private readonly BinaryFormatter binaryFormatter;
        private readonly IMemoryStreamManager memoryStreamManager;

        public virtual byte[] Serialize<T>(T value) where T : class
        {
            if(value == null)
                return Array.Empty<byte>();

            using (var memoryStream = memoryStreamManager.GetStream()){
                binaryFormatter.Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        public virtual T Deserialize<T>(byte[] value) where T : class
        {
            if (value == null || value.Length == 0)
                return default;

            using (var memoryStream = memoryStreamManager.GetStream(value)){
                return binaryFormatter.Deserialize(memoryStream) as T;
            }
        }

        public BinarySerializer(IMemoryStreamManager memoryStreamManager)
        {
            
            binaryFormatter = new BinaryFormatter();
            this.memoryStreamManager = memoryStreamManager;
        }
    }
}
