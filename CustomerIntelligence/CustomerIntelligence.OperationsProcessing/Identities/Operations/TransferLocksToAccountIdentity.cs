using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class TransferLocksToAccountIdentity : OperationIdentityBase<TransferLocksToAccountIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.TransferLocksToAccountIdentity; }
        }

        public override string Description
        {
            get { return "Перенос блокировок на другой ЛС"; }
        }
    }
}