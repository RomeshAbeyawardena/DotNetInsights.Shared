using System;

namespace DotNetInsights.Shared.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdentityAttribute: Attribute
    {
        public bool IsIdentityColumn { get; }

        public IdentityAttribute(bool isIdentityColumn = true)
        {
            IsIdentityColumn = isIdentityColumn;
        }
    }
}