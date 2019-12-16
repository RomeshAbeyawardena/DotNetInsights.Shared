using System;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Services
{
    public class MessagePackBinarySerializer : IMessagePackBinarySerializer
    {
        private readonly IMemoryStreamManager memoryStreamManager;

        public byte[] Serialize<T>(T value) where T : class
        {
            if(value == null)
                return Array.Empty<byte>();

            using (var memoryStream = memoryStreamManager.GetStream())
            {
                MessagePack.MessagePackSerializer.Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] value) where T : class
        {
            if (value == null || value.Length == 0)
                return default;

            using (var memoryStream = memoryStreamManager.GetStream(value)){
                return MessagePack.MessagePackSerializer.Deserialize<T>(memoryStream);
            }
        }

        public MessagePackBinarySerializer(IMemoryStreamManager memoryStreamManager)
        {
            this.memoryStreamManager = memoryStreamManager;
        }

        
    }
}