using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public sealed class EntityTypeFirmCategory : EntityTypeBase<EntityTypeFirmCategory>
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