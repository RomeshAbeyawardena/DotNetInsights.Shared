using System;

namespace DotNetInsights.Shared.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EncryptionKeyAttribute : Attribute
    {
        public EncryptionKeyAttribute(bool isEncryptionKey = true)
        {
            IsEncryptionKey = isEncryptionKey;
        }

        public bool IsEncryptionKey { get; }
    }
}