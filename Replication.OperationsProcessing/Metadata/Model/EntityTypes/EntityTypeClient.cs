using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public sealed class EntityTypeClient : EntityTypeBase<EntityTypeClient>
    {
        public override int Id
        {
            get { return EntityTypeIds.Client; }
        }

        public override string Description
        {
            get { return "Client"; }
        }
    }
}
