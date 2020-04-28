using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AnamSoft.Cache
{
    /// <summary>
    /// Represents the base class for generic cache.
    /// </summary>
    /// <typeparam name="TKey">The type of a key.</typeparam>
    /// <typeparam name="TValue">The type of a value.</typeparam>    
    public abstract class CacheBase<TKey, TValue> : ICache<TKey, TValue>, IDisposable where TKey : notnull
    {
        protected IDictionary<TKey, CacheItem<TValue>> Items;
        private bool _disposed;

        protected CacheBase(IDictionary<TKey, CacheItem<TValue>> items, string name = "default")
        {
            Items = items;
            Name = name;
        }

        public int Count
        {
            get
            {
                if (_disposed)
                    DisposedError();

                return Items.Count;
            }
        }

        public string Name { get; }

        public void Add(TKey key, TValue value, CacheItemPolicy? policy)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            AddTrusted(key, value, policy);
        }

        public void AddOrSet(TKey key, TValue value, CacheItemPolicy? policy)
        {
            if (!TryAdd(key, value, policy))
                Set(key, value, policy);
        }

        public bool TryAdd(TKey key, TValue value, CacheItemPolicy? policy)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            if (Items.ContainsKey(key))
                return false;

            AddTrusted(key, value, policy);
            return true;
        }

        public void Set(TKey key, TValue value, CacheItemPolicy? policy)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            SetTrusted(key, value, policy);
        }

        public TValue Get(TKey key)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            return Items[key].Value;
        }

        public TValue Get(TKey key, TValue defaultValue)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            return Items.TryGetValue(key, out var item) ? item.Value : defaultValue;
        }

        public bool TryGetValue(TKey key, [MaybeNull] out TValue value)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            if (Items.TryGetValue(key, out var item))
            {
                value = item.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool Remove([DisallowNull] TKey key)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            return Items.Remove(key);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key is null)
                    NullError(nameof(key));

                return Get(key);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (_disposed)
                DisposedError();

            if (key is null)
                NullError(nameof(key));

            return Items.ContainsKey(key);
        }

        #region Enumerable
        public IEnumerator<KeyValuePair<TKey, CacheItem<TValue>>> GetEnumerator()
        {
            if (_disposed)
                DisposedError();

            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region Disposing
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //Items = null;
            }
            _disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        #endregion

        protected virtual void AddTrusted([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy)
            => Items.Add(key, new CacheItem<TValue>(value, policy) { Cached = true });

        protected virtual void SetTrusted([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy)
         => Items[key] = new CacheItem<TValue>(value, policy) { Cached = true };

        [DoesNotReturn] protected void DisposedError() => throw new ObjectDisposedException(Name);

        [DoesNotReturn] protected void NullError(string name) => throw new ArgumentNullException(name);
    }
}
