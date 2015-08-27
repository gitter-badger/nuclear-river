using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class CopyOrderIdentity : OperationIdentityBase<CopyOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CopyOrderIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Copy order";
            }
        }
    }
}
