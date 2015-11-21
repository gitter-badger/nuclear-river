using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class ApplyOrderDiscountIdentity : OperationIdentityBase<ApplyOrderDiscountIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ApplyOrderDiscountIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Apply order discount";
            }
        }
    }
}
