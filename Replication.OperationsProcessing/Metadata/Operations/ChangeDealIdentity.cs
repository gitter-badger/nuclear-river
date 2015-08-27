using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
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