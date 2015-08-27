using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public class EntityTypeOrder : EntityTypeBase<EntityTypeOrder>
    {
        public override int Id
        {
            get { return EntityTypeIds.Order; }
        }

        public override string Description
        {
            get { return "Order"; }
        }
    }
}