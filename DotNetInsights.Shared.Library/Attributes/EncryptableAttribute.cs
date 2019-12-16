using System;

namespace DotNetInsights.Shared.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EncryptableAttribute : Attribute
    {
        public EncryptableAttribute(bool encryptField = true)
        {
            EncryptField = encryptField;
        }

        public bool EncryptField { get; }
    }
}