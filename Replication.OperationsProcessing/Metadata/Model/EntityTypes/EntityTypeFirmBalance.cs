using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

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