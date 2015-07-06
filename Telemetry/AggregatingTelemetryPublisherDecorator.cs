using System.Collections.Generic;

namespace NuClear.Telemetry
{
    public sealed class AggregatingTelemetryPublisherDecorator : ITelemetryPublisher
    {
        private readonly IEnumerable<ITelemetryPublisher> _profilers;

        public AggregatingTelemetryPublisherDecorator(IEnumerable<ITelemetryPublisher> profilers)
        {
            _profilers = profilers;
        }

        public void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            foreach (var profiler in _profilers)
            {
                profiler.Publish<T>(value);
            }
        }

        public void Trace(string message, object data = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            foreach (var profiler in _profilers)
            {
                profiler.Trace(message, data, memberName, sourceFilePath, sourceLineNumber);
            }
        }
    }
}