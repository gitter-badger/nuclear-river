using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ChangeRequisitesIdentity : OperationIdentityBase<ChangeRequisitesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeLegalPersonRequisitesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Изменить реквизиты юрлица";
            }
        }
    }
}