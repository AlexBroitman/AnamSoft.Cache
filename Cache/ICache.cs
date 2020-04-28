using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AnamSoft.Cache
{
    public interface ICache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, CacheItem<TValue>>> where TKey : notnull
    {
        string Name { get; }

        int Count { get; }

        void Add(TKey key, TValue value, CacheItemPolicy? policy = default);

        void AddOrSet(TKey key, TValue value, CacheItemPolicy? policy = default);

        bool TryAdd(TKey key, TValue value, CacheItemPolicy? policy = default);

        TValue Get(TKey key);

        TValue Get(TKey key, TValue defaultValue);

        bool TryGetValue(TKey key, out TValue value);

        void Set(TKey key, TValue value, CacheItemPolicy? policy = default);

        bool Remove(TKey key);

        TValue this[TKey key] { get; }

        bool ContainsKey(TKey key);
    }
}
