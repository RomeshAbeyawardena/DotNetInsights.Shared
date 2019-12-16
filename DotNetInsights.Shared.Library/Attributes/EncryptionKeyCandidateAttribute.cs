using System;
using System.Globalization;

namespace DotNetInsights.Shared.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EncryptionKeyCandidateAttribute : Attribute
    {
        public EncryptionKeyCandidateAttribute(string format = "{0}", bool isEncryptionKey = true)
        {
            Format = format;
            IsEncryptionKey = isEncryptionKey;
            FormatProvider = CultureInfo.CurrentCulture;
        }

        public IFormatProvider FormatProvider { get; }
        public string Format { get; }
        public bool IsEncryptionKey { get; }
    }
}