using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public class EntityTypeActivity : EntityTypeBase<EntityTypeActivity>
    {
        public override int Id
        {
            get { return EntityTypeIds.Activity; }
        }

        public override string Description
        {
            get { return "Activity"; }
        }
    }
}