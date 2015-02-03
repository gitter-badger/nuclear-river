using System.Data.Entity;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.QueryExecution;

namespace NuClear.AdvancedSearch.Web.OData.DataAccess
{
    public sealed class ODataDbContext : DbContext
    {
        public ODataDbContext(IEdmModel model, ODataConnectionFactory connectionFactory)
            : base(connectionFactory.CreateConnection(model.GetMetadataIdentity()), model.GetDbCompiledModel(), true)
        {
            Database.CommandTimeout = 60;
        }
    }
}