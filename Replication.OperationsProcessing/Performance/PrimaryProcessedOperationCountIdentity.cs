using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class PrimaryProcessedOperationCountIdentity : PerformanceIdentityBase<PrimaryProcessedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return "PrimaryProcessedOperationCountIdentity"; }
        }

        public override string Description
        {
            get { return " оличество обработанных операций на этапе primary"; }
        }
    }
}