using System.Collections.Generic;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class AggregateProfiler : IProfiler
    {
        private readonly IEnumerable<IProfiler> _profilers;

        public AggregateProfiler(IEnumerable<IProfiler> profilers)
        {
            _profilers = profilers;
        }

        public AggregateProfiler(IProfiler p1, IProfiler p2, IProfiler p3)
            : this(new[] { p1, p2, p3 })
        {
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