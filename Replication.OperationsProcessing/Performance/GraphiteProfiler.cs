using System;

using Graphite;

using NuClear.AdvancedSearch.Settings;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class GraphiteProfiler : IProfiler
    {
        private readonly IEnvironmentSettings _environmentSettings;

        static GraphiteProfiler()
        {
            try
            {
                StaticMetricsPipeProvider.Instance.Start();
            }
            catch (Exception)
            {
            }
        }

        public GraphiteProfiler(IEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            var path = string.Format("{0}.{1}.{2}", _environmentSettings.EntryPointName, _environmentSettings.EnvironmentName, PerformanceIdentityBase<T>.Instance.Name);
            MetricsPipe.Current.Raw(path, value);
        }
    }
}