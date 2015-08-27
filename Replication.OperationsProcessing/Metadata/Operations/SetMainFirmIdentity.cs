using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class SetMainFirmIdentity : OperationIdentityBase<SetMainFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SetMainFirmIdentity; }
        }

        public override string Description
        {
            get { return "Смена главной фирмы у клиента"; }
        }
    }
}