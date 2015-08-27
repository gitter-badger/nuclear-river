using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ImportCardIdentity : OperationIdentityBase<ImportCardIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCardsFromServiceBusIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowCards.Card"; }
        }
    }
}