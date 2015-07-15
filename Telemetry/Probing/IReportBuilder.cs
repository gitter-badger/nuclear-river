namespace NuClear.Telemetry.Probing
{
    public interface IReportBuilder
    {
        IReport Build(ProbeWatcher root);
    }
}