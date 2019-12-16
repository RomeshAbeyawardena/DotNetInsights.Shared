using DotNetInsights.Shared.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services
{
    public static class DefaultSwitch
    {
        public static ISwitch<TKey, TValue> Create<TKey, TValue>()
        {
            return new Switch<TKey, TValue>();
        }
    }

    public sealed class Switch<TKey, TValue> : ISwitch<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _switchDictionary;
        private readonly IDictionary<TKey, TKey> _aliasDictionary;
        public Switch()
        {
            _switchDictionary = new Dictionary<TKey, TValue>();
            _aliasDictionary = new Dictionary<TKey, TKey>();
        }

        public TValue this[TKey key] => _switchDictionary[key];

        public IEnumerable<TKey> Keys => _switchDictionary.Keys;

        public IEnumerable<TValue> Values => _switchDictionary.Values;

        public int Count => _switchDictionary.Count;

        public TValue Case(TKey key)
        {
            if(_switchDictionary.TryGetValue(key, out var value))
                return value;

            if(_aliasDictionary.ContainsKey(key))
                return Case(_aliasDictionary[key]);

            throw new NullReferenceException($"Unable to find value for {key}");
        }

        public ISwitch<TKey, TValue> CaseWhen(TKey key, TValue value, params TKey[] aliases)
        {
            if(ContainsKey(key))
                throw new NullReferenceException("A value for {key} already exists");

            foreach(var alias in aliases)
            {
                if(!_aliasDictionary.ContainsKey(alias))
                    _aliasDictionary.Add(alias, key);
            }

            _switchDictionary.Add(key, value);
            return this;
        }

        public bool ContainsKey(TKey key)
        {
            return _switchDictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _switchDictionary.GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _switchDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _switchDictionary.GetEnumerator();
        }
    }
}
