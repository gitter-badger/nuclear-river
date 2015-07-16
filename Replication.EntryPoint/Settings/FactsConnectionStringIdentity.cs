using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public class FactsConnectionStringIdentity : IdentityBase<FactsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 2; }
        }

        public override string Description
        {
            get { return "Facts DB connection string"; }
        }
    }
}