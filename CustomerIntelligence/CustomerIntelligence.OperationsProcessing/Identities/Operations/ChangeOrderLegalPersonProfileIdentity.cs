using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
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