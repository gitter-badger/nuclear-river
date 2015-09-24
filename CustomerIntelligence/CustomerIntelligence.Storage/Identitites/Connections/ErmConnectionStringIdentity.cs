using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Storage.Identitites.Connections
{
    public class ErmConnectionStringIdentity : IdentityBase<ErmConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 1; }
        }

        public override string Description
        {
            get { return "Erm DB connnection string"; }
        }
    }
}