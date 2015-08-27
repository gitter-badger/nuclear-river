using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    public sealed class ImportFirmPromisingIdentity : OperationIdentityBase<ImportFirmPromisingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportFirmPromising;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт перспективности фирм";
            }
        }
    }
}