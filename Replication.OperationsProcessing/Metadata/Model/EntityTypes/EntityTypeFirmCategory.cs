using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeFirmCategory : EntityTypeBase<EntityTypeFirmCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategory; }
        }

        public override string Description
        {
            get { return "FirmCategory"; }
        }
    }
}