using System;
using System.ComponentModel;

using Graphite;

using NuClear.AdvancedSearch.Settings;

namespace NuClear.Telemetry
{
    public sealed class GraphiteTelemetryPublisher : ITelemetryPublisher
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IGraphiteCounterMetadata _provider;

        static GraphiteTelemetryPublisher()
        {
            try
            {
                StaticMetricsPipeProvider.Instance.Start();
            }
            catch (Exception)
            {
            }
        }

        public GraphiteTelemetryPublisher(IEnvironmentSettings environmentSettings, IGraphiteCounterMetadata provider)
        {
            _environmentSettings = environmentSettings;
            _provider = provider;
        }

        public void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            string path;
            GraphiteMetadataElement metadata;
            if (_provider == null || (metadata = _provider.Get<T>()) == null)
            {
                path = string.Format("{0}.{1}.{2}", _environmentSettings.EntryPointName, _environmentSettings.EnvironmentName, TelemetryIdentityBase<T>.Instance.Name);
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

        public void Trace(string message, object data)
        {
            throw new NotImplementedException();
        }
    }
}