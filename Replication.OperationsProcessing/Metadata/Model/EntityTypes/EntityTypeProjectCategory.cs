using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeProjectCategory : EntityTypeBase<EntityTypeProjectCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.ProjectCategory; }
        }

        public override string Description
        {
            get { return "ProjectCategory"; }
        }
    }
}