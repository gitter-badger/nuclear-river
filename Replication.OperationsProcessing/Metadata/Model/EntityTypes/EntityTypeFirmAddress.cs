using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public sealed class EntityTypeFirmAddress : EntityTypeBase<EntityTypeFirmAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmAddress; }
        }

        public override string Description
        {
            get { return "FirmAddress"; }
        }
    }
}