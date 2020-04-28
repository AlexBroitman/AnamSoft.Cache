using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AnamSoft.Cache
{
    public interface IConcurrentCache<TKey, TValue> : ICache<TKey, TValue>, IEnumerable<KeyValuePair<TKey, CacheItem<TValue>>> where TKey : notnull
    {
        TValue GetOrAdd(TKey key, TValue value, CacheItemPolicy? policy = default);

        TValue GetOrAdd(TKey key, Func<TKey, TValue> newValueFactory, CacheItemPolicy? policy = default);

        TValue AddOrUpdate(TKey key, Func<TKey, TValue> newValueFactory, Func<TKey, TValue, TValue> updateValueFactory, CacheItemPolicy? policy = default);
    }
}
