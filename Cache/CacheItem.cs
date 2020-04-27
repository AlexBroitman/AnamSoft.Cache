using System.Diagnostics.CodeAnalysis;

namespace Cache
{
    public readonly struct CacheItem<TValue>
    {
        [NotNull]
        public TValue Value { get; }

        public CacheItemPolicy Policy { get; }

        public CacheItem([DisallowNull] TValue value, CacheItemPolicy? policy = null)
        {
            Value = value;
            Policy = policy is null ? Policy = CacheItemPolicy.Default : policy;
        }
    }
}
