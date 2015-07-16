using System;
using System.Runtime.Remoting.Messaging;

namespace NuClear.Telemetry.Probing
{
    /// <summary>
    /// Реализует входную точку для сбора данных по продолжительности операций.
    /// </summary>
    public static class Probe
    {
        private static IReportBuilder _reportBuilder = new ReportBuilder();
        private static IReportSink _reportSink = DefaultReportSink.Instance;

        public static IReportBuilder ReportBuilder 
        {
            get { return _reportBuilder; }
            set { _reportBuilder = value; }
        }

        public static IReportSink ReportSink
        {
            get { return _reportSink; }
            set { _reportSink = value; }
        }

        private static ProbeWatcher Ambient
        {
            get { return (ProbeWatcher)CallContext.LogicalGetData("AmbientProbe"); }
            set { CallContext.LogicalSetData("AmbientProbe", value); }
        }

        /// <summary>
        /// Создаёт scope измеряемой операции
        /// </summary>
        public static IDisposable Create(string name, params string[] nameItems)
        {
            return Create(name + " " + string.Join(" ", nameItems));
        }

        /// <summary>
        /// Создаёт scope измеряемой операции
        /// </summary>
        public static IDisposable Create(string name)
        {
            var parent = Ambient;
            var probe = new ProbeWatcher(name, parent, Complete);
            if (parent != null)
            {
                parent.Childs.Add(probe);
            }

            return probe;
        }

        private static void Complete(ProbeWatcher completed)
        {
            Ambient = completed.Parent;

            if (completed.Parent == null)
            {
                var report = ReportBuilder.Build(completed);
                ReportSink.Push(report);
            }
        }
    }
}
