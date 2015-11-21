using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class ChangeDealIdentity : OperationIdentityBase<ChangeDealIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeDealIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Сменить сделку";
            }
        }
    }
}