using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public sealed class EntityTypeAccount : EntityTypeBase<EntityTypeAccount>
    {
        public override int Id
        {
            get { return EntityTypeIds.Account; }
        }

        public override string Description
        {
            get { return "Account"; }
        }
    }
}