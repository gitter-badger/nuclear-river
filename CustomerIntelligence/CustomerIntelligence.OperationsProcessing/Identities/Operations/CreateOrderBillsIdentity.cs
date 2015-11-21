using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
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