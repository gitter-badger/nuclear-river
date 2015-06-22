using System;
using System.Collections.Generic;
using System.ComponentModel;

using Graphite;

using NuClear.AdvancedSearch.Settings;
using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public sealed class GraphiteProfiler : IProfiler
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IGraphiteCounterMetadata _provider;

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

        public GraphiteProfiler(IEnvironmentSettings environmentSettings, IGraphiteCounterMetadata provider)
        {
            _environmentSettings = environmentSettings;
            _provider = provider;
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            string path;
            GraphiteMetadataElement metadata;
            if (_provider == null || (metadata = _provider.Get<T>()) == null)
            {
                path = string.Format("{0}.{1}.{2}", _environmentSettings.EntryPointName, _environmentSettings.EnvironmentName, PerformanceIdentityBase<T>.Instance.Name);
                MetricsPipe.Current.Gauge(path, value);
                return;
            }

            path = string.Format("{0}.{1}.{2}", _environmentSettings.EntryPointName, _environmentSettings.EnvironmentName, metadata.Name);
            switch (metadata.Type)
            {
                case GraphiteMetadataElement.CounterType.Gauge:
                    MetricsPipe.Current.Gauge(path, value);
                    return;
                case GraphiteMetadataElement.CounterType.Counter:
                    MetricsPipe.Current.Count(path, value);
                    return;
                case GraphiteMetadataElement.CounterType.Timer:
                    MetricsPipe.Current.Timing(path, value);
                    return;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}