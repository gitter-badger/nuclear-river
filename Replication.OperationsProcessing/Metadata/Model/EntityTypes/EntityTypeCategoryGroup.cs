using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeCategoryGroup : EntityTypeBase<EntityTypeCategoryGroup>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryGroup; }
        }

        public override string Description
        {
            get { return "CategoryGroup"; }
        }
    }
}