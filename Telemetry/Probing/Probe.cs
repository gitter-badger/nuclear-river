using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NuClear.Telemetry.Probing
{
    public sealed class Probe : IDisposable
    {
        private static readonly ThreadLocal<Probe> Ambient = new ThreadLocal<Probe>();
        private readonly string _name;
        private readonly Stopwatch _watch;
        private readonly List<Probe> _childs;
        private readonly Probe _parent;

        public Probe(string name)
        {
            _name = name;
            _watch = Stopwatch.StartNew();
            _childs = new List<Probe>();
            _parent = Ambient.Value;
            Ambient.Value = this;
        }

        public void Dispose()
        {
            _watch.Stop();
            Ambient.Value = _parent;

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
