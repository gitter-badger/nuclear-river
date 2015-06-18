namespace NuClear.Telemetry
{
    public interface IProfiler
    {
        void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new();
    }
}