using System.Collections.Generic;
using System.Diagnostics;

using NuClear.AdvancedSearch.Settings;
using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public sealed class PerformanceCounterTelemetryPublisher : ITelemetryPublisher
    {
        private static readonly IDictionary<string, PerformanceCounter> Counters = new Dictionary<string, PerformanceCounter>();
        private readonly IEnvironmentSettings _environmentSettings;

        public PerformanceCounterTelemetryPublisher(IEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        public void Report<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            try
            {
                var counter = GetCounter(_environmentSettings.EnvironmentName, IdentityBase<T>.Instance.Name);
                counter.RawValue = value;
            }
            catch
            {
                // FIXME {a.rechkalov, 18.06.2015}: пофиксить ecpetion
            }
        }

        private static PerformanceCounter GetCounter(string environmentName, string counterName)
        {
            PerformanceCounter counter;
            if (!Counters.TryGetValue(counterName, out counter))
            {
                counter = new PerformanceCounter
                          {
                              CategoryName = "Erm",
                              CounterName = counterName,
                              InstanceName = environmentName,
                              ReadOnly = false,
                              InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                          };
                Counters.Add(counterName, counter);
            }

            return counter;
        }
    }
}