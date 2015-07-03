using System.Collections.Generic;

namespace NuClear.Telemetry
{
    public sealed class AggregateTelemetry : ITelemetry
    {
        private readonly IEnumerable<ITelemetry> _profilers;

        public AggregateTelemetry(IEnumerable<ITelemetry> profilers)
        {
            _profilers = profilers;
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            foreach (var profiler in _profilers)
            {
                profiler.Report<T>(value);
            }
        }
    }
}