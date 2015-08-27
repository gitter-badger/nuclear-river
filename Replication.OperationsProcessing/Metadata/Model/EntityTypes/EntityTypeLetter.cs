using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public class EntityTypeLetter : EntityTypeBase<EntityTypeLetter>
    {
        public override int Id
        {
            get { return EntityTypeIds.Letter; }
        }

        public override string Description
        {
            get { return "Letter"; }
        }
    }
}