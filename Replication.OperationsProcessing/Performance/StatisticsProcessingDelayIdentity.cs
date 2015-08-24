using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class StatisticsProcessingDelayIdentity : TelemetryIdentityBase<StatisticsProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Отставание обработки сообщений пересчёта статистики"; }
        }
    }
}