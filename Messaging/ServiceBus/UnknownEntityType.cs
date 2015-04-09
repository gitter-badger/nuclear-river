using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Messaging.ServiceBus
{
    public sealed class UnknownEntityType : EntityTypeBase<UnknownEntityType>
    {
        private int _id;

        public override int Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Unknown EntityType with Id=" + Id; }
        }

        public UnknownEntityType SetId(int id)
        {
            _id = id;
            return this;
        }
    }
}