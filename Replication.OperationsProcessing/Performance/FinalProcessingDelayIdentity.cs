using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class FinalProcessingDelayIdentity : PerformanceIdentityBase<FinalProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "ќтставание обработки сообщений от генерации"; }
        }
    }
}