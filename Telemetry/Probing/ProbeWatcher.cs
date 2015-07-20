using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NuClear.Telemetry.Probing
{
    public sealed class ProbeWatcher : IDisposable
    {
        private readonly Action<ProbeWatcher> _action;
        private readonly Stopwatch _watch;

        internal ProbeWatcher(string name, ProbeWatcher parent, Action<ProbeWatcher> action)
        {
            _action = action;
            _watch = Stopwatch.StartNew();
            Childs = new List<ProbeWatcher>();
            Name = name;
            Parent = parent;
        }

        public string Name { get; private set; }
        public ProbeWatcher Parent { get; private set; }
        public List<ProbeWatcher> Childs { get; private set; }
        public long ElapsedMilliseconds
        {
            get { return _watch.ElapsedMilliseconds; }
        }

        public void Dispose()
        {
            _watch.Stop();
            _action.Invoke(this);
        }
    }
}