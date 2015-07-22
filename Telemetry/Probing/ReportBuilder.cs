using System.Collections.Generic;
using System.Linq;

namespace NuClear.Telemetry.Probing
{
    internal sealed class ReportBuilder : IReportBuilder
    {
        private static IReportSink _reportSink = DefaultReportSink.Instance;

        public static IReportSink ReportSink
        {
            get { return _reportSink; }
            set { _reportSink = value; }
        }

        public void Build(ProbeWatcher root)
        {
            var report = Build(new[] { root });
            ReportSink.Push(report);
        }

        private static Report Build(IEnumerable<ProbeWatcher> sameTypeProbes)
        {
            var probes = sameTypeProbes.ToList();
            return new Report
                   {
                       Name = probes.First().Name,
                       Count = probes.Count,
                       ElapsedMilliseconds = probes.Sum(x => x.ElapsedMilliseconds),
                       Subactivities = probes.SelectMany(x => x.Childs).GroupBy(x => x.Name).Select(Build).ToArray()
                   };
        }
    }
}