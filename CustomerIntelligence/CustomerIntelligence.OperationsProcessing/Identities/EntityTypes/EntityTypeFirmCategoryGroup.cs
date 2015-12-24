using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirmCategoryGroup : EntityTypeBase<EntityTypeFirmCategoryGroup>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategoryGroup; }
        }

        public override string Description
        {
            get { return "FirmCategoryGroup"; }
        }
    }
}