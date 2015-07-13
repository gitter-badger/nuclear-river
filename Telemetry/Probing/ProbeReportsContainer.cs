using System;
using System.Collections.Generic;

namespace NuClear.Telemetry.Probing
{
    public sealed class ProbeReportsContainer
    {
        private static readonly Lazy<ProbeReportsContainer> Singleton = new Lazy<ProbeReportsContainer>(() => new ProbeReportsContainer());
        private List<Probe.Report> _continer;

        private ProbeReportsContainer()
        {
            _continer = new List<Probe.Report>();
        }

        public static ProbeReportsContainer Instance
        {
            get { return Singleton.Value; }
        }

        public void Add(Probe.Report report)
        {
            lock (_continer)
            {
                _continer.Add(report);
            }
        }

        public IEnumerable<object> GetReports()
        {
            lock (_continer)
            {
                var result = _continer;
                _continer = new List<Probe.Report>();
                return result;
            }
        }
    }
}
