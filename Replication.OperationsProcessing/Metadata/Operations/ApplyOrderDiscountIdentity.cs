using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
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
