using System.Collections.Generic;
using System.Diagnostics;

using NuClear.AdvancedSearch.Settings;
using NuClear.Model.Common;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class PerformanceCounterProfiler : IProfiler
    {
        private static readonly IDictionary<string, PerformanceCounter> Counters = new Dictionary<string, PerformanceCounter>();
        private readonly IEnvironmentSettings _environmentSettings;

        public PerformanceCounterProfiler(IEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            var counter = GetCounter(_environmentSettings.EnvironmentName, IdentityBase<T>.Instance.Name);
            counter.RawValue = value;
        }

        private PerformanceCounter GetCounter(string environmentName, string counterName)
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