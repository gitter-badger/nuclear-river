using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class ImportAdvModelInRubricInfoIdentity : OperationIdentityBase<ImportAdvModelInRubricInfoIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportAdvModelInRubricInfoIdentity; }
        }

        public override string Description
        {
            get { return "Импорт потока сообщения AdvModelInRubricInfo"; }
        }
    }
}