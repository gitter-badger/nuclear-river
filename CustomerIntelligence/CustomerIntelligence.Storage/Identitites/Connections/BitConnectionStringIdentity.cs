using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Storage.Identitites.Connections
{
    public class BitConnectionStringIdentity : IdentityBase<BitConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 5; }
        }

        public override string Description
        {
            get { return "Bit facts DB connection string identity"; }
        }
    }
}