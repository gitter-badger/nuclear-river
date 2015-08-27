using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    public sealed class EntityTypeAppointment : EntityTypeBase<EntityTypeAppointment>
    {
        public override int Id
        {
            get { return EntityTypeIds.Appointment; }
        }

        public override string Description
        {
            get { return "Appointment"; }
        }
    }
}