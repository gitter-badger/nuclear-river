using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public class PrimaryProcessingQueueLengthIdentity : PerformanceIdentityBase<PrimaryProcessingQueueLengthIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Размер очереди ETL1"; }
        }
    }
}