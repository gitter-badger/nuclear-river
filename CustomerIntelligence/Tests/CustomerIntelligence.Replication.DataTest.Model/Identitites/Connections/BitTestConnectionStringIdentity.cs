using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class BitTestConnectionStringIdentity : IdentityBase<BitTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 8; }
        }

        public override string Description
        {
            get { return "Bit facts DB connection string identity (state initialization testing scope)"; }
        }
    }
}