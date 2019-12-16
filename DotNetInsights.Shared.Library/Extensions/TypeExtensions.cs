using System;
using System.Reflection;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class TypeExtensions
    {
        public static void ForProperties(this Type type, object source, object instance,
            Action<PropertyInfo, object, object> forPropertiesAction)
        {
            var sourceType = source.GetType();
            var properties =  sourceType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                forPropertiesAction(propertyInfo, source, instance);
            }
        }
    }
}