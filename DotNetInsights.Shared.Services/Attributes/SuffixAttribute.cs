using System;

namespace DotNetInsights.Shared.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SuffixAttribute : Attribute
    {
        public SuffixAttribute(string suffix)
        {
            Suffix = suffix;
        }

        public string Suffix { get; }
    }
}
