using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class BitStatisticsEntityProcessedCountIdentity : PerformanceIdentityBase<BitStatisticsEntityProcessedCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return " оличество прин€тых записей статистики из Bit"; }
        }
    }
}