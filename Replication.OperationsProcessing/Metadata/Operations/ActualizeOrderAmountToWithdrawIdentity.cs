using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    [DataContract]
    public sealed class ActualizeOrderAmountToWithdrawIdentity : OperationIdentityBase<ActualizeOrderAmountToWithdrawIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ActualizeOrderAmountToWithdrawIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Актуализировать сумму к списанию по заказу";
            }
        }
    }
}