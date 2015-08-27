using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class UpdateOrderFinancialPerformanceIdentity : OperationIdentityBase<UpdateOrderFinancialPerformanceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.UpdateOrderFinancialPerformanceIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Обновление стоимости заказа и его позиций";
            }
        }
    }
}
