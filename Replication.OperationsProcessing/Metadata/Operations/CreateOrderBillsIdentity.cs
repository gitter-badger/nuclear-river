using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class CreateOrderBillsIdentity : OperationIdentityBase<CreateOrderBillsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CreateOrderBillsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Создание счетов по заказу";
            }
        }
    }
}