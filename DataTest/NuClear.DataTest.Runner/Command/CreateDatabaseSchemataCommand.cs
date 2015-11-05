using System;
using System.Linq;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using Microsoft.SqlServer.Management.Smo;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;

namespace NuClear.DataTest.Runner.Command
{
    public sealed class CreateDatabaseSchemataCommand : Command
    {
        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly SmoConnectionFactory _smoConnectionFactory;

        public CreateDatabaseSchemataCommand(Assembly targetAssembly, IMetadataProvider metadataProvider, DataConnectionFactory dataConnectionFactory, SmoConnectionFactory smoConnectionFactory)
            : base(targetAssembly, metadataProvider)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _smoConnectionFactory = smoConnectionFactory;
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            var database = _smoConnectionFactory.CreateDatabaseConnection(metadataElement.ConnectionStringIdentity);
            using (var dataConnection = _dataConnectionFactory.CreateConnection(metadataElement))
            {
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
    }
}