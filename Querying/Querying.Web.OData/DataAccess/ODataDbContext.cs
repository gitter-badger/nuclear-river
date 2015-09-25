using System.Data.Entity;

using Microsoft.OData.Edm;

using NuClear.Querying.QueryExecution;

namespace NuClear.Querying.Web.OData.DataAccess
{
    public sealed class ODataDbContext : DbContext
    {
        static ODataDbContext()
        {
            // disable DbContext type cache
            Database.SetInitializer(new NullDatabaseInitializer<ODataDbContext>());
        }

        public ODataDbContext(IEdmModel model, ODataConnectionFactory connectionFactory)
            : base(connectionFactory.CreateConnection(model.GetMetadataIdentity()), model.GetDbCompiledModel(), true)
        {
            Database.CommandTimeout = 600;
        }
    }
}