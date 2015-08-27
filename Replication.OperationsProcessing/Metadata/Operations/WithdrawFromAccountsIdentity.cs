using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    [DataContract]
    public sealed class WithdrawFromAccountsIdentity : OperationIdentityBase<WithdrawFromAccountsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.WithdrawFromAccountsIdentity; }
        }

        public override string Description
        {
            get { return "Make withdraw from accounts"; }
        }
    }
}
