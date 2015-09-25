using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace NuClear.AdvancedSearch.Web.OData.DataAccess
{
    public sealed class ODataDbConfiguration : DbConfiguration
    {
        public ODataDbConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }
}