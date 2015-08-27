using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ClearOrderBargainIdentity : OperationIdentityBase<ClearOrderBargainIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ClearOrderBargainIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Удаление договора в заказе";
            }
        }
    }
}