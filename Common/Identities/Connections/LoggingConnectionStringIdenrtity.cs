using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.AdvancedSearch.Common.Identities.Connections
{
    public class LoggingConnectionStringIdenrtity : IdentityBase<LoggingConnectionStringIdenrtity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 7; }
        }

        public override string Description
        {
            get { return "Logging storage connection string identity"; }
        }
    }
}