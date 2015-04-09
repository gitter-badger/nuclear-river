using NuClear.Model.Common.Operations.Identity;

namespace NuClear.AdvancedSearch.Messaging.ServiceBus
{
    public sealed class UnknownOperationIdentity : OperationIdentityBase<UnknownOperationIdentity>
    {
        private int _id;

        public override int Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Unknown OperationIdentity with Id=" + Id; }
        }

        internal UnknownOperationIdentity SetId(int id)
        {
            _id = id;
            return this;
        }
    }
}