using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Caching;

namespace AnamSoft.Cache
{
    public class ReadOnlyChangeMonitorCollection : ReadOnlyCollection<ChangeMonitor>
    {
        internal ReadOnlyChangeMonitorCollection(IList<ChangeMonitor> changeMonitors) : base(changeMonitors) { }
    }
}