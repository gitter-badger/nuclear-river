using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class AggregateEnquiedOperationCountIdentity : TelemetryIdentityBase<AggregateEnquiedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество взятых на обработку операций над агрегатами"; }
        }
    }
}