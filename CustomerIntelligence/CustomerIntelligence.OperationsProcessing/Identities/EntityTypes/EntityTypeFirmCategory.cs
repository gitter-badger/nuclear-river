using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirmCategory : EntityTypeBase<EntityTypeFirmCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategory; }
        }

        public override string Description
        {
            get { return "FirmCategory"; }
        }
    }
}