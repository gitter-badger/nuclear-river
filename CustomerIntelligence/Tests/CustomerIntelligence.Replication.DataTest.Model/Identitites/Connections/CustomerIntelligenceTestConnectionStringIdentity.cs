using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class CustomerIntelligenceTestConnectionStringIdentity : IdentityBase<CustomerIntelligenceTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 9; }
        }

        public override string Description
        {
            get { return "CustomerIntelligence DB connection string identity (state initialization testing scope)"; }
        }
    }
}