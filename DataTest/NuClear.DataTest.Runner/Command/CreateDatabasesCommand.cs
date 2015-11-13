using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Smo;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner.Command
{
    public sealed class CreateDatabasesCommand : Command
    {
        private readonly ConnectionStringSettingsAspect _settings;
        private readonly SmoConnectionFactory _smoConnectionFactory;

        public CreateDatabasesCommand(IMetadataProvider metadataProvider, ConnectionStringSettingsAspect connectionStringSettings, SmoConnectionFactory smoConnectionFactory)
            : base(metadataProvider)
        {
            _settings = connectionStringSettings;
            _smoConnectionFactory = smoConnectionFactory;
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            var connectionString = _settings.GetConnectionString(metadataElement.ConnectionStringIdentity);
            var server = _smoConnectionFactory.CreateServerConnection(metadataElement.ConnectionStringIdentity);
            var targetDbName = GetDatabaseName(connectionString);
            var existingDb = server.Databases[targetDbName];
            if (existingDb != null)
            {
                return;
            }

            var newDb = new Database(server, targetDbName);
            newDb.Create();
        }

        private string GetDatabaseName(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }
    }
}