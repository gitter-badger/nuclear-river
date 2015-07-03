namespace NuClear.Telemetry
{
    public interface ITelemetry
    {
        void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new();
    }
}