using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace AnamSoft.Cache
{
    /// <summary>
    /// Represents the generic concurrent cache. The class is thread safe.
    /// </summary>
    /// <inheritdoc/> 
    public class ConcurrentCache<TKey, TValue> : CacheBase<TKey, TValue>, IConcurrentCache<TKey, TValue>, IDisposable where TKey : notnull
    {
        private ConcurrentDictionary<TKey, CacheItem<TValue>> _items;
        private bool _disposed;

        public ConcurrentCache(string name = "default") : base(new ConcurrentDictionary<TKey, CacheItem<TValue>>(), name)
        {
            _items = (ConcurrentDictionary<TKey, CacheItem<TValue>>)Items;
        }

        public TValue GetOrAdd(TKey key, TValue value, CacheItemPolicy? policy = null)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            return GetOrAddTrusted(key, value, policy);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> newValueFactory, CacheItemPolicy? policy = null)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (newValueFactory is null)
                NullError(nameof(newValueFactory));

            return GetOrAddTrusted(key, newValueFactory, policy);
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> newValueFactory, Func<TKey, TValue, TValue> updateValueFactory, CacheItemPolicy? policy = null)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (newValueFactory is null)
                NullError(nameof(newValueFactory));

            if (updateValueFactory is null)
                NullError(nameof(updateValueFactory));

            return AddOrUpdateTrusted(key, newValueFactory, updateValueFactory, policy);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // dispose managed resources here
            }

            // dispose unmanaged resources here

            _disposed = true;

            base.Dispose(disposing);
        }

        protected TValue GetOrAddTrusted(TKey key, TValue value, CacheItemPolicy? policy = null)
            => _items.GetOrAdd(key, key => new CacheItem<TValue>(value, policy)).Value;

        protected TValue GetOrAddTrusted(TKey key, Func<TKey, TValue> newValueFactory, CacheItemPolicy? policy)
            => _items.GetOrAdd(key, key => new CacheItem<TValue>(newValueFactory(key), policy)).Value;

        protected TValue AddOrUpdateTrusted(TKey key, Func<TKey, TValue> newValueFactory, Func<TKey, TValue, TValue> updateValueFactory, CacheItemPolicy? policy = null)
        {
            CacheItem<TValue> NewCacheItemFactory(TKey key) => new CacheItem<TValue>(newValueFactory(key), policy);
            CacheItem<TValue> UpdateCacheItemFactory(TKey key, CacheItem<TValue> ci) => new CacheItem<TValue>(updateValueFactory(key, ci.Value), policy);

            var cacheItem = _items.AddOrUpdate(key, NewCacheItemFactory, UpdateCacheItemFactory);

            return cacheItem.Value;
        }
    }
}
