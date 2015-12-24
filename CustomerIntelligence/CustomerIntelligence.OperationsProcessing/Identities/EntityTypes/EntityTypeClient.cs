using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
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
