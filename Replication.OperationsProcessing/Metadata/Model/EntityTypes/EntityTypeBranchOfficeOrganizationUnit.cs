using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeBranchOfficeOrganizationUnit : EntityTypeBase<EntityTypeBranchOfficeOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.BranchOfficeOrganizationUnit; }
        }

        public override string Description
        {
            get { return "BranchOfficeOrganizationUnit"; }
        }
    }
}