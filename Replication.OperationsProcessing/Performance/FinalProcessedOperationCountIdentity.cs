using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class FinalProcessedOperationCountIdentity : PerformanceIdentityBase<FinalProcessedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return "FinalProcessedOperationCountIdentity"; }
        }

        public override string Description
        {
            get { return " оличество обработанных операций на этапе final"; }
        }
    }
}