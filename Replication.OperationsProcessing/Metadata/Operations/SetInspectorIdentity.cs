using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class SetInspectorIdentity : OperationIdentityBase<SetInspectorIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SetInspectorIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Указать проверяющего";
            }
        }
    }
}