using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
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