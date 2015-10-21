using System;
using System.Data.SqlClient;
using System.Reflection;

using Microsoft.SqlServer.Management.Smo;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner.Command
{
    public sealed class CreateDatabasesCommand : Command
    {
        private readonly ConnectionStringSettingsAspect _settings;

        public CreateDatabasesCommand(Assembly targetAssembly, IMetadataProvider metadataProvider)
            : base(targetAssembly, metadataProvider)
        {
            _settings = TargetAssembly.GetConnectionStrings();
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            var connectionString = _settings.GetConnectionString(metadataElement.ConnectionStringIdentity);
            var server = ConnectToServer(connectionString);
            var targetDbName = GetDatabaseName(connectionString);
            var existingDb = server.Databases[targetDbName];
            if (existingDb != null)
            {
                throw new Exception($"Database {connectionString} already exists");
            }

            var newDb = new Database(server, targetDbName);
            newDb.Create();
        }

        private string GetDatabaseName(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }

        private Server ConnectToServer(string connectionString)
        {
            var server = new Server();
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "" };
            server.ConnectionContext.ConnectionString = connectionStringBuilder.ConnectionString;
            if (!server.ConnectionContext.IsOpen)
            {
                server.ConnectionContext.Connect();
            }

            return server;
        }
    }
}