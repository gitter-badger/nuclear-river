using System;

using NuClear.AdvancedSearch.Settings;
using NuClear.Tracing.API;

namespace NuClear.Telemetry.Zabbix
{
    public sealed class ZabbixTelemetryPublisher : ITelemetryPublisher, IDisposable
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly ITracer _tracer;
        private readonly ZabbixSender _zabbixSender;

        public ZabbixTelemetryPublisher(IEnvironmentSettings environmentSettings, IZabbixSettings zabbixSettings, ITracer tracer)
        {
            _environmentSettings = environmentSettings;
            _tracer = tracer;
            _zabbixSender = new ZabbixSender(zabbixSettings.ZabbixUri);
        }

        public async void Report<T>(long value) where T : TelemetryIdentityBase<T>, new()
        {
            var zabbixItem = new ZabbixItem
            {
                Host = _environmentSettings.EnvironmentName,
                ItemKey = string.Format("{0}.{1}", _environmentSettings.EntryPointName, TelemetryIdentityBase<T>.Instance.Name),
                Value = value,
            };

            var response = await _zabbixSender.Send(new[] { zabbixItem });
            if (!response.Success)
            {
                _tracer.Warn("Cannot send profiling data to Zabbix");
            }
        }

        public void Dispose()
        {
            _zabbixSender.Dispose();
        }
    }
}