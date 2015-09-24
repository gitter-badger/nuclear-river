using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Identities.Telemetry
{
    public sealed class ErmEnqueuedOperationCountIdentity : TelemetryIdentityBase<ErmEnqueuedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return " оличество вз€тых на обработку CUD операций"; }
        }
    }
}