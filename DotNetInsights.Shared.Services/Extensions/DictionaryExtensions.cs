using DotNetInsights.Shared.Contracts.Builders;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DotNetInsights.Shared.Services.Extensions
{
    public static class DictionaryExtensions
    {
        public static object ToObject<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            dynamic expandoObject = dictionary as ExpandoObject;
            
            return expandoObject;
        }

        public static object ToObject<TKey, TValue>(this IDictionaryBuilder<TKey, TValue> dictionary)
        {
            dynamic expandoObject = dictionary as ExpandoObject;

            return expandoObject;
        }

        public static IEnumerable<TValue> TryGetValues<TKey, TValue>(this IDictionary<TKey, TValue> values, params TKey[] keys)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            if(keys == null)
                throw new ArgumentNullException(nameof(keys));

            foreach(var key in keys)
            {
                if(values.TryGetValue(key, out var value))
                    yield return value;
            }
        }
    }
}
