using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ChangeOrderLegalPersonIdentity : OperationIdentityBase<ChangeOrderLegalPersonIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeOrderLegalPersonIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена юр. лица клиента в заказе";
            }
        }
    }
}