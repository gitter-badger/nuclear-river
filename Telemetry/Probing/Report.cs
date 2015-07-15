using System.Collections.Generic;

namespace NuClear.Telemetry.Probing
{
    internal sealed class Report : IReport
    {
        public string Name { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public int Count { get; set; }
        public IEnumerable<Report> Subactivities { get; set; }
    }
}