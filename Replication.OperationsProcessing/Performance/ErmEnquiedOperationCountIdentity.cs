using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ErmEnquiedOperationCountIdentity : TelemetryIdentityBase<ErmEnquiedOperationCountIdentity>
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