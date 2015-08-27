using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public class EntityTypeCategoryOrganizationUnit : EntityTypeBase<EntityTypeCategoryOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryOrganizationUnit; }
        }

        public override string Description
        {
            get { return "CategoryOrganizationUnit"; }
        }
    }
}