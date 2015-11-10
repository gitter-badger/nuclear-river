using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class ActualizeActiveLocksIdentity : OperationIdentityBase<ActualizeActiveLocksIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeActiveLocksIdentity; }
        }

        public override string Description
        {
            get { return "Актуалиазция блокировок при пересчете планируемых списаний"; }
        }
    }
}