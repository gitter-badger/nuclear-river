using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeTerritory : EntityTypeBase<EntityTypeTerritory>
    {
        public override int Id
        {
            get { return EntityTypeIds.Territory; }
        }

        public override string Description
        {
            get { return "Territory"; }
        }
    }
}