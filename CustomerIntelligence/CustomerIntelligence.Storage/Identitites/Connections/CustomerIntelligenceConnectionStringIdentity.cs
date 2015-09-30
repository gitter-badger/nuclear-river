using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Storage.Identitites.Connections
{
    public class TransportConnectionStringIdentity : IdentityBase<TransportConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 4; }
        }

        public override string Description
        {
            get { return "CustomerIntelligence DB connection string identity"; }
        }
    }
}