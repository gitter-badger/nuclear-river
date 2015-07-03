using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ErmOperationCountIdentity : TelemetryIdentityBase<ErmOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество принятых CUD операций из ERM"; }
        }
    }
}