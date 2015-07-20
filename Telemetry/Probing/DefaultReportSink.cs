using System;
using System.Collections.Generic;

namespace NuClear.Telemetry.Probing
{
    public sealed class DefaultReportSink : IReportSink
    {
        private static readonly Lazy<DefaultReportSink> SingleInstance = new Lazy<DefaultReportSink>(() => new DefaultReportSink());

        private List<IReport> _continer;

        private DefaultReportSink()
        {
            _continer = new List<IReport>();
        }

        public static DefaultReportSink Instance
        {
            get { return SingleInstance.Value; }
        }

        public void Push(IReport report)
        {
            lock (_continer)
            {
                _continer.Add(report);
            }
        }

        public IEnumerable<IReport> ConsumeReports()
        {
            lock (_continer)
            {
                var result = _continer;
                _continer = new List<IReport>();
                return result;
            }
        }
    }
}
