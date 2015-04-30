using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeContact : EntityTypeBase<EntityTypeContact>
    {
        public override int Id
        {
            get { return EntityTypeIds.Contact; }
        }

        public override string Description
        {
            get { return "Contact"; }
        }
    }
}