using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeCategory : EntityTypeBase<EntityTypeCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.Category; }
        }

        public override string Description
        {
            get { return "Category"; }
        }
    }
}