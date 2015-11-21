using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class CloseWithDenialIdentity : OperationIdentityBase<CloseWithDenialIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CloseWithDenialIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Закрыть отказом";
            }
        }
    }
}