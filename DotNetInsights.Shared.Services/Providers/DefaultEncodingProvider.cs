using System;
using DotNetInsights.Shared.Contracts.Providers;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Services.Providers
{
    public class DefaultEncodingProvider : IEncodingProvider
    {
        private readonly ISwitch<string, Encoding> encodingDictionary;

        public IEnumerable<Encoding> Encodings => new [] { 
            Encoding.ASCII, 
            Encoding.UTF7, 
            Encoding.UTF8, 
            Encoding.UTF32, 
            Encoding.Unicode, 
            Encoding.BigEndianUnicode 
        };

        public Encoding GetEncoding(IEnumerable<Encoding> encodings, string encodingName)
        {
            var encoding = encodingDictionary.Case(encodingName);

            if(encoding == null)
                return GetEncodingByInternalName(encodings, encodingName);

            return encoding;
        }

        internal Encoding GetEncodingByInternalName(IEnumerable<Encoding> encodings, string encodingName)
        {
            return encodings.SingleOrDefault(encoding => 
                encoding.EncodingName.Equals(encodingName, StringComparison.InvariantCultureIgnoreCase) || 
                encoding.WebName.Equals(encodingName, StringComparison.InvariantCultureIgnoreCase) || 
                encoding.BodyName.Equals(encodingName, StringComparison.InvariantCultureIgnoreCase) );
        }

        public DefaultEncodingProvider(ISwitch<string, Encoding> encodingDictionary)
        {
            this.encodingDictionary = encodingDictionary;
        }
    }
}
