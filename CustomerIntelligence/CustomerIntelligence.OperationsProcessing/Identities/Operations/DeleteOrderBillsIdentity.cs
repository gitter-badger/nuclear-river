using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class DeleteOrderBillsIdentity : OperationIdentityBase<DeleteOrderBillsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DeleteOrderBillsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Удаление счетов по заказу";
            }
        }
    }
}