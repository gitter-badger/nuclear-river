using NuClear.Model.Common.Operations.Identity;

namespace NuClear.AdvancedSearch.Replication.API.Operations.Identities
{
    public class InitializeAggregateOperationIdentity : OperationIdentityBase<InitializeAggregateOperationIdentity>
    {
        public override int Id
        {
            get { return 1; }
        }

        public override string Description
        {
            get { return "Initialize aggregate operation"; }
        }
    }
}