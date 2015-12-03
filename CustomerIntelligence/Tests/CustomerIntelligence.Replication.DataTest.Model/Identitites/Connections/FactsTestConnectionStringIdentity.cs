using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class FactsTestConnectionStringIdentity : IdentityBase<FactsTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 12; }
        }

        public override string Description
        {
            get { return "Facts DB connection string (state initialization testing scope)"; }
        }
    }
}