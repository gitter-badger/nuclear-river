using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace NuClear.AdvancedSearch.Common.Identities.Connections
{
    public class CustomerIntelligenceConnectionStringIdentity : IdentityBase<CustomerIntelligenceConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 3; }
        }

        public override string Description
        {
            get { return "Operations transport DB connection string identity"; }
        }
    }
}