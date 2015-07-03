namespace NuClear.Telemetry
{
    public interface ITelemetry
    {
        void Report<T>(long value)
            where T : TelemetryIdentityBase<T>, new();
    }
}