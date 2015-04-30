using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public class EntityTypeLegalPerson : EntityTypeBase<EntityTypeLegalPerson>
    {
        public override int Id
        {
            get { return EntityTypeIds.LegalPerson; }
        }

        public override string Description
        {
            get { return "LegalPerson"; }
        }
    }
}