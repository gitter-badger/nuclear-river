using System.Collections.Generic;

namespace NuClear.Telemetry
{
    public sealed class AggregateTelemetryPublisher : ITelemetryPublisher
    {
        private readonly IEnumerable<ITelemetryPublisher> _profilers;

        public AggregateTelemetryPublisher(IEnumerable<ITelemetryPublisher> profilers)
        {
            _profilers = profilers;
        }

        public void Report<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            foreach (var profiler in _profilers)
            {
                profiler.Report<T>(value);
            }
        }
    }
}