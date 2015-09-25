using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
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
            get { return "Unknown OperationIdentity"; }
        }

        internal UnknownOperationIdentity SetId(int id)
        {
            _id = id;
            return this;
        }
    }
}