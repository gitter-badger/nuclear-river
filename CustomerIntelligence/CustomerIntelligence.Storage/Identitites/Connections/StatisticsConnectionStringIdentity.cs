using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Storage.Identitites.Connections
{
    public class StatisticsConnectionStringIdentity : IdentityBase<StatisticsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 6; }
        }

        public override string Description
        {
            get { return "Statistics DB connection string identity"; }
        }
    }
}