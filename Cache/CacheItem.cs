using System.Diagnostics.CodeAnalysis;

namespace AnamSoft.Cache
{
    public readonly struct CacheItem<TValue>
    {
        public TValue Value { get; }

        public CacheItemPolicy Policy { get; }

        public CacheItem(TValue value, CacheItemPolicy? policy = null)
        {
            Value = value;
            Policy = policy ?? CacheItemPolicy.Default;
        }

        internal bool Cached { get => Policy.Cached; set => Policy.Cached = value; }
    }
}
