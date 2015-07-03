namespace NuClear.Telemetry
{
    public interface ITelemetryPublisher
    {
        void Report<T>(long value)
            where T : TelemetryIdentityBase<T>, new();
    }
}