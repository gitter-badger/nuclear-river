using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.AdvancedSearch.Common.Identities.Connections
{
    public class TransportConnectionStringIdentity : IdentityBase<TransportConnectionStringIdentity>, IConnectionStringIdentity
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