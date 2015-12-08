using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeProjectCategory : EntityTypeBase<EntityTypeProjectCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.ProjectCategory; }
        }

        public override string Description
        {
            get { return "ProjectCategory"; }
        }
    }
}