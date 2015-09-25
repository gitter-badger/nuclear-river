using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Identities.Telemetry
{
    public sealed class StatisticsEnqueuedOperationCountIdentity : TelemetryIdentityBase<StatisticsEnqueuedOperationCountIdentity>
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