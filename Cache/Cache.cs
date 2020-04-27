using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cache
{
    public interface ICache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, CacheItem<TValue>>>
    {
        string Name { get; }
        int Count { get; }
        void Add([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy = default);
        void AddOrSet([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy = default);
        bool TryAdd([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy = default);
        [return: NotNull] TValue Get([DisallowNull] TKey key);
        [return: MaybeNull] TValue Get([DisallowNull] TKey key, [AllowNull] TValue defaultValue);
        bool TryGet([DisallowNull] TKey key, [AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out TValue value);
        void Set([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy = default);
        bool Remove([DisallowNull] TKey key);
        [NotNull] TValue this[[DisallowNull] TKey key] { get; }
        bool ContainsKey([DisallowNull] TKey key);
    }

    /// <summary>
    /// Represents the generic cache. The class is not thread safe.
    /// </summary>
    /// <typeparam name="TKey">The type of a key.</typeparam>
    /// <typeparam name="TValue">The type of a value.</typeparam>    
    public class Cache<TKey, TValue> : ICache<TKey, TValue>, IDisposable
    {
        protected Dictionary<TKey, CacheItem<TValue>> Items;
        private bool _disposed;

        public Cache(string name = "default")
        {
            Items = new Dictionary<TKey, CacheItem<TValue>>();
            Name = name;
        }

        public int Count
        {
            get
            {
                if (_disposed)
                    ExposedError();

                return Items.Count;
            }
        }

        public string Name { get; }

        public void Add([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy)
        {
            if (_disposed)
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            Items.Add(key, new CacheItem<TValue>(value, policy));
        }

        public void AddOrSet([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy)
        {
            if (!TryAdd(key, value, policy))
                Set(key, value, policy);
        }

        public bool TryAdd([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy)
        {
            if (_disposed)
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            if (Items.ContainsKey(key))
                return false;

            Items.Add(key, new CacheItem<TValue>(value, policy));
            return true;
        }

        public void Set([DisallowNull] TKey key, [DisallowNull] TValue value, CacheItemPolicy? policy)
        {
            if (_disposed)
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            if (value is null)
                NullError(nameof(value));

            Items[key] = new CacheItem<TValue>(value, policy);
        }

        [return: NotNull]
        public TValue Get([DisallowNull] TKey key)
        {
            if (_disposed)
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            return Items[key].Value;
        }

        [return: MaybeNull]
        public TValue Get([DisallowNull] TKey key, [AllowNull] TValue defaultValue)
        {
            if (_disposed)
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            return Items.TryGetValue(key, out var item) ? item.Value : defaultValue;
        }

        public bool TryGet([DisallowNull] TKey key, [AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out TValue value)
        {
            if (_disposed)
                ExposedError();

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
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            return Items.Remove(key);
        }

        [NotNull, DisallowNull]
        public TValue this[[DisallowNull]TKey key]
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
                ExposedError();

            if (key is null)
                NullError(nameof(key));

            return Items.ContainsKey(key);
        }

        #region Enumerable
        public IEnumerator<KeyValuePair<TKey, CacheItem<TValue>>> GetEnumerator()
        {
            if (_disposed)
                ExposedError();

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

        [DoesNotReturn] private void ExposedError() => throw new ObjectDisposedException(Name);

        [DoesNotReturn] private void NullError(string name) => throw new ArgumentNullException(name);
    }
}
