namespace NuClear.Telemetry.Probing
{
    public interface IReportSink
    {
        void Push(IReport report);
    }
}