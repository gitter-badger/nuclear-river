namespace NuClear.Telemetry.Probing
{
    public interface IReportBuilder
    {
        void Build(ProbeWatcher root);
    }
}