using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ChangeOrderLegalPersonProfileIdentity : OperationIdentityBase<ChangeOrderLegalPersonProfileIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeOrderLegalPersonProfileIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена профилей в заказе";
            }
        }
    }
}