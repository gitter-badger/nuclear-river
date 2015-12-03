using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class ErmMasterConnectionStringIdentity : IdentityBase<ErmMasterConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 10; }
        }

        public override string Description
        {
            get { return "Erm Master DB connnection string (state initialization testing scope)"; }
        }
    }
}