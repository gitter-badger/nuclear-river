using System.Collections.Generic;

namespace NuClear.Telemetry
{
    public sealed class AggregateProfiler : IProfiler
    {
        private readonly IEnumerable<IProfiler> _profilers;

        public AggregateProfiler(IEnumerable<IProfiler> profilers)
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