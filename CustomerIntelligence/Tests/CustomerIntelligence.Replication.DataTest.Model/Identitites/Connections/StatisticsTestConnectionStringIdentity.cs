using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class StatisticsTestConnectionStringIdentity : IdentityBase<StatisticsTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 13; }
        }

        public override string Description
        {
            get { return "Statistics DB connection string identity (state initialization testing scope)"; }
        }
    }
}