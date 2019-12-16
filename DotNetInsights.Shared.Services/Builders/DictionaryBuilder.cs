using DotNetInsights.Shared.Contracts.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetInsights.Shared.Services.Builders
{
    public static class DictionaryBuilder
    {
        public static IDictionaryBuilder<TKey, TValue> Create<TKey, TValue>()
        {
            return new DictionaryBuilder<TKey, TValue>();
        }
    }

    public sealed class DictionaryBuilder<TKey, TValue> : IDictionaryBuilder<TKey, TValue>
    {
        public TValue this[TKey key] => _dictionary[key];

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public IDictionaryBuilder<TKey, TValue> Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair);
            return this;
        }

        public IDictionaryBuilder<TKey, TValue> Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            return this;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public IDictionaryBuilder<TKey, TValue> Remove(TKey key)
        {
            _dictionary.Remove(key);
            return this;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public IDictionary<TKey, TValue> ToDictionary()
        {
            return _dictionary.ToDictionary(dictionary => dictionary.Key, dictionary => dictionary.Value);
        }

        public DictionaryBuilder()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        private readonly IDictionary<TKey, TValue> _dictionary;
    }

}
