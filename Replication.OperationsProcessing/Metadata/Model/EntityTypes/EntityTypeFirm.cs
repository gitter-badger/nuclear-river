using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeFirm : EntityTypeBase<EntityTypeFirm>
    {
        public override int Id
        {
            get { return EntityTypeIds.Firm; }
        }

        public override string Description
        {
            get { return "Firm"; }
        }
    }
}