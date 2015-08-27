using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class CalculateClientPromisingIdentity : OperationIdentityBase<CalculateClientPromisingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CalculateClientPromisingIdentity; }
        }

        public override string Description
        {
            get { return "Операция пересчета перспективности клиентов"; }
        }
    }
}