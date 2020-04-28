using System.Collections.Generic;

namespace AnamSoft.Cache
{
    /// <summary>
    /// Represents the generic cache. The class is not thread safe.
    /// </summary>
    /// <inheritdoc/>
    public class Cache<TKey, TValue> : CacheBase<TKey, TValue> where TKey : notnull
    {
        public Cache(string name = "default") : base(new Dictionary<TKey, CacheItem<TValue>>(), name) { }
    }
}
