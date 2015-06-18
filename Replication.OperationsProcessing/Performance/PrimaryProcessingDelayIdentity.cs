using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class PrimaryProcessingDelayIdentity : PerformanceIdentityBase<PrimaryProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return "PrimaryProcessingDelay"; }
        }

        public override string Description
        {
            get { return "ќтставание обработки сообщений от генерации"; }
        }
    }
}