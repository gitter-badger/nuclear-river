using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace NuClear.AdvancedSearch.Replication.API.Identitites.Connections
{
    public class CustomerIntelligenceConnectionStringIdentity : IdentityBase<CustomerIntelligenceConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 3; }
        }

        public override string Description
        {
            get { return "CustomerIntelligence DB connection string identity"; }
        }
    }
}