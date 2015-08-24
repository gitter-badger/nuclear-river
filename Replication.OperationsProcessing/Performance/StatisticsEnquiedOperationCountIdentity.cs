using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class StatisticsEnquiedOperationCountIdentity : TelemetryIdentityBase<StatisticsEnquiedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return " оличество операций пересчЄта статистики, добавленных в очередь на обработку"; }
        }
    }
}