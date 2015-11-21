using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.AdvancedSearch.Common.Identities.Connections
{
    public class ServiceBusConnectionStringIdentity : IdentityBase<ServiceBusConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 5; }
        }

        public override string Description
        {
            get { return "MS Service Bus connection string identity"; }
        }
    }
}