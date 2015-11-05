using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Smo;

using NuClear.DataTest.Metamodel;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner
{
    public sealed class SmoConnectionFactory
    {
        private readonly ConnectionStringSettingsAspect _connectionStringSettings;

        public SmoConnectionFactory(ConnectionStringSettingsAspect connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public Database CreateDatabaseConnection(IConnectionStringIdentity connectionStringIdentity)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(connectionStringIdentity);
            var server = new Server();
            server.ConnectionContext.ConnectionString = WithoutInitialCatalog(connectionString);
            if (!server.ConnectionContext.IsOpen)
            {
                server.ConnectionContext.Connect();
            }

            return server.Databases[GetDatabaseName(connectionString)];
        }

        public Server CreateServerConnection(IConnectionStringIdentity connectionStringIdentity)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(connectionStringIdentity);
            var server = new Server();
            server.ConnectionContext.ConnectionString = WithoutInitialCatalog(connectionString);
            if (!server.ConnectionContext.IsOpen)
            {
                server.ConnectionContext.Connect();
            }

            return server;
        }

        private string GetDatabaseName(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }

        private string WithoutInitialCatalog(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString) { InitialCatalog = string.Empty }.ConnectionString;
        }
    }
}
