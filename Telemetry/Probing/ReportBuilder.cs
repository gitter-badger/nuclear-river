using System.Collections.Generic;
using System.Linq;

namespace NuClear.Telemetry.Probing
{
    internal sealed class ReportBuilder : IReportBuilder
    {
        public IReport Build(ProbeWatcher root)
        {
            return Build(new[] { root });
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