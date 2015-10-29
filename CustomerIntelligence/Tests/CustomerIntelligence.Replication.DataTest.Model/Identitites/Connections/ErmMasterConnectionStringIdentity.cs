using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace CustomerIntelligence.Replication.DataTest.Model.Identitites.Connections
{
    public sealed class ErmMasterConnectionStringIdentity : IdentityBase<ErmMasterConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 6; }
        }

        public override string Description
        {
            get { return "Erm Master DB connnection string"; }
        }
    }
}