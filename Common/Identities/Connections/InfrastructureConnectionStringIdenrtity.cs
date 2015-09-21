using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace NuClear.AdvancedSearch.Common.Identities.Connections
{
    public class InfrastructureConnectionStringIdenrtity : IdentityBase<InfrastructureConnectionStringIdenrtity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 6; }
        }

        public override string Description
        {
            get { return "Infrastructure DB connections string identity"; }
        }
    }
}