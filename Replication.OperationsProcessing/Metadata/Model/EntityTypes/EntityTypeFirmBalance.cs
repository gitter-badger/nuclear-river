using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeFirmBalance : EntityTypeBase<EntityTypeFirmBalance>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmBalance; }
        }

        public override string Description
        {
            get { return "FirmBalance"; }
        }
    }
}