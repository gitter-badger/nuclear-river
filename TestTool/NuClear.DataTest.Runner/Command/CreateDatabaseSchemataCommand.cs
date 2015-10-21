using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using Microsoft.SqlServer.Management.Smo;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner.Command
{
    public sealed class CreateDatabaseSchemataCommand : Command
    {
        private readonly ConnectionStringSettingsAspect _settings;

        public CreateDatabaseSchemataCommand(Assembly targetAssembly, IMetadataProvider metadataProvider)
            : base(targetAssembly, metadataProvider)
        {
            _settings = TargetAssembly.GetConnectionStrings();
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            var connectionString = _settings.GetConnectionString(metadataElement.ConnectionStringIdentity);
            var database = ConnectToDatabase(connectionString);
            using (var dataConnection = SqlServerTools.CreateDataConnection(connectionString))
            {
                dataConnection.AddMappingSchema(new MappingSchema(new SqlServerMappingSchema(), metadataElement.Schema));
                var schemaNames = metadataElement.Entities
                                                 .Select(x => dataConnection.MappingSchema.GetAttribute<TableAttribute>(x)?.Schema)
                                                 .Where(x => !string.IsNullOrEmpty(x) && database.Schemas[x] == null)
                                                 .Distinct()
                                                 .ToArray();

                foreach (var schemaName in schemaNames)
                {
                    var schema = new Schema(database, schemaName);
                    schema.Create();
                }

                foreach (var entity in metadataElement.Entities)
                {
                    var factory = (ITableFactory)Activator.CreateInstance(typeof(TableFactory<>).MakeGenericType(entity));
                    factory.CreateTable(dataConnection);
                }
            }
        }

        private interface ITableFactory
        {
            void CreateTable(DataConnection dataConnection);
        }

        private class TableFactory<T> : ITableFactory
        {
            public void CreateTable(DataConnection dataConnection)
            {
                dataConnection.CreateTable<T>();
            }
        }

        private Database ConnectToDatabase(string connectionString)
        {
            var server = new Server();
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            server.ConnectionContext.ConnectionString = connectionStringBuilder.ConnectionString;
            if (!server.ConnectionContext.IsOpen)
            {
                server.ConnectionContext.Connect();
            }

            return server.Databases[connectionStringBuilder.InitialCatalog];
        }
    }
}