using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ErmOperationCountIdentity : PerformanceIdentityBase<ErmOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return "ErmOperationCountIdentity"; }
        }

        public override string Description
        {
            get { return "Количество принятых CUD операций из ERM"; }
        }
    }
}