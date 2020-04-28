using System;
using System.Runtime.Caching;

namespace AnamSoft.Cache
{
    public interface ICacheItemPolicy
    {
        DateTime AbsoluteExpiration { get; }

        TimeSpan SlidingExpiration { get; }

        int Priority { get; set; }

        ReadOnlyChangeMonitorCollection ChangeMonitors { get; }

        void AddChangeMonitor(ChangeMonitor changeMonitor);

        void RemoveChangeMonitor(ChangeMonitor changeMonitor);

        event EventHandler<EventArgs>? Added;
        event EventHandler<EventArgs>? Adding;
        event EventHandler<EventArgs>? Removed;
        event EventHandler<EventArgs>? Removing;
        event EventHandler<EventArgs>? Updated;
        event EventHandler<EventArgs>? Updating;
    }
}