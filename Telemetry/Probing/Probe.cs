using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace NuClear.Telemetry.Probing
{
    public sealed class Probe : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _watch;
        private readonly List<Probe> _childs;
        private readonly Probe _parent;

        public Probe(string name)
        {
            _name = name;
            _watch = Stopwatch.StartNew();
            _childs = new List<Probe>();
            _parent = Ambient;
            Ambient = this;
        }

        private static Probe Ambient
        {
            get { return (Probe)CallContext.LogicalGetData("AmbientProbe"); }
            set { CallContext.LogicalSetData("AmbientProbe", value); }
        }

        public void Dispose()
        {
            _watch.Stop();
            Ambient = _parent;

            if (_parent == null)
            {
                var report = BuildReport(new[] { this });
                ProbeReportsContainer.Instance.Add(report);
            }
            else
            {
                _parent.AddChild(this);
            }
        }
        
        private static Report BuildReport(IEnumerable<Probe> sameTypeProbes)
        {
            var probes = sameTypeProbes.ToList();
            return new Report
            {
                Name = probes.First()._name,
                Count = probes.Count,
                ElapsedMilliseconds = probes.Sum(x => x._watch.ElapsedMilliseconds),
                Subactivities = probes.SelectMany(x => x._childs).GroupBy(x => x._name).Select(BuildReport).ToArray()
            };
        }

        private void AddChild(Probe probe)
        {
            _childs.Add(probe);
        }

        public class Report
        {
            public string Name { get; set; }
            public long ElapsedMilliseconds { get; set; }
            public int Count { get; set; }
            public IEnumerable<Report> Subactivities { get; set; }
        }
    }
}
