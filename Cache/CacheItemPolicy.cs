using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Cache
{
    public class CacheItemPolicy : IEquatable<CacheItemPolicy?>
    {
        public static CacheItemPolicy Default = new CacheItemPolicy(TimeSpan.FromMinutes(1));

        public DateTime AbsoluteExpiration { get; }

        public TimeSpan SlidingExpiration { get; }

        public int Priority { get; set; }

        public ReadOnlyChangeMonitorCollection ChangeMonitors { get; }

        private CacheItemPolicy()
        {
            _changeMonitors = new List<ChangeMonitor>();
            ChangeMonitors = new ReadOnlyChangeMonitorCollection(_changeMonitors);
        }

        public CacheItemPolicy(DateTime absoluteExpiration) : this()
        {
            if (absoluteExpiration < DateTime.UtcNow)
                throw new ArgumentOutOfRangeException(nameof(absoluteExpiration), absoluteExpiration, "AbsoluteExpiration must be in future.");

            AbsoluteExpiration = absoluteExpiration;
        }

        public CacheItemPolicy(TimeSpan slidingExpiration) : this()
        {
            if (slidingExpiration == default)
                throw new ArgumentOutOfRangeException(nameof(slidingExpiration), slidingExpiration, "SlidingExpiration must be greater than zero.");

            SlidingExpiration = slidingExpiration;
        }

        public event EventHandler<EventArgs>? Adding;
        public event EventHandler<EventArgs>? Added;
        public event EventHandler<EventArgs>? Updating;
        public event EventHandler<EventArgs>? Updated;
        public event EventHandler<EventArgs>? Removing;
        public event EventHandler<EventArgs>? Removed;

        protected virtual void OnAdding(EventArgs ea) => Adding?.Invoke(this, ea);
        protected virtual void OnAdded(EventArgs ea) => Added?.Invoke(this, ea);
        protected virtual void OnUpdating(EventArgs ea) => Updating?.Invoke(this, ea);
        protected virtual void OnUpdated(EventArgs ea) => Updated?.Invoke(this, ea);
        protected virtual void OnRemoving(EventArgs ea) => Removing?.Invoke(this, ea);
        protected virtual void OnRemoved(EventArgs ea) => Removed?.Invoke(this, ea);

        private List<ChangeMonitor> _changeMonitors;

        #region Equality
        public static bool operator ==(CacheItemPolicy? a, CacheItemPolicy? b)
            => ReferenceEquals(a, b) || !(a is null) && !(b is null) && a.SlidingExpiration == b.SlidingExpiration && a.AbsoluteExpiration == b.AbsoluteExpiration;

        public static bool operator !=(CacheItemPolicy? a, CacheItemPolicy? b) => !(a == b);

        public bool Equals(CacheItemPolicy? other) => this == other;

        public override bool Equals(object? obj) => obj is CacheItemPolicy other && this == other;

        public override int GetHashCode() => HashCode.Combine(AbsoluteExpiration, SlidingExpiration);
        #endregion
    }
}
