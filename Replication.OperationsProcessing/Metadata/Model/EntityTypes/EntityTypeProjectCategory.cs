using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model;

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