using System;
using System.Collections.Concurrent;
using System.Diagnostics;

using NuClear.AdvancedSearch.Settings;
using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public sealed class PerformanceCounterTelemetryPublisher : ITelemetryPublisher
    {
        private static readonly ConcurrentDictionary<string, PerformanceCounter> Counters = new ConcurrentDictionary<string, PerformanceCounter>();
        private readonly IEnvironmentSettings _environmentSettings;

        public PerformanceCounterTelemetryPublisher(IEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        public void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            try
            {
                var counter = GetCounter(_environmentSettings.EnvironmentName, IdentityBase<T>.Instance.Name);
                counter.RawValue = value;
            }
            catch(Exception)
            {
            }
        }

        public void Trace(string message, object data = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        private static PerformanceCounter GetCounter(string environmentName, string counterName)
        {
            return Counters.GetOrAdd(counterName,
                                     key => new PerformanceCounter
                                            {
                                                CategoryName = "Erm",
                                                CounterName = key,
                                                InstanceName = environmentName,
                                                ReadOnly = false,
                                                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                                            });
        }
    }
}