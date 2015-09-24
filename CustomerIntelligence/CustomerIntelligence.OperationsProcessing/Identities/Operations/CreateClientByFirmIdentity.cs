using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public sealed class CreateClientByFirmIdentity : OperationIdentityBase<CreateClientByFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CreateClientByFirmIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Создание клиента фирмы";
            }
        }
    }
}