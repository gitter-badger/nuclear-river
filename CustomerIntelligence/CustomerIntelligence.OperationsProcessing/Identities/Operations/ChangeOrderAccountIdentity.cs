using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class ChangeOrderAccountIdentity : OperationIdentityBase<ChangeOrderAccountIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeOrderAccountIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена ЛС в заказе";
            }
        }
    }
}