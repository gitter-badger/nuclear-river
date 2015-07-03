namespace NuClear.Telemetry
{
    public interface ITelemetryPublisher
    {
        void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new();
    }
}