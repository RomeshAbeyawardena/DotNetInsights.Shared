using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts.Builders
{
    public interface IDictionaryBuilder<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryBuilder<TKey, TValue> Add(KeyValuePair<TKey, TValue> keyValuePair);
        IDictionaryBuilder<TKey, TValue> Add(TKey key, TValue value);
        IDictionaryBuilder<TKey, TValue> Remove(TKey key);
        IDictionary<TKey, TValue> ToDictionary();
    }
}
