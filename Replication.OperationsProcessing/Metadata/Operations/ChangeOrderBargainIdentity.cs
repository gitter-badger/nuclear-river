using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ChangeOrderBargainIdentity : OperationIdentityBase<ChangeOrderBargainIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeOrderBargainIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена договора в заказе";
            }
        }
    }
}