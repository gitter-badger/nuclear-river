using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.CustomerIntelligence.Replication.Tests.Data;
using NuClear.Storage.API.Readings;
using NuClear.Storage.Readings;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    public class Loader : ILoader, IDisposable
    {
        private readonly string _sourceContextName;
        private readonly string _targetContextName;
        private readonly MappingSchema _sourceContextSchema;
        private readonly MappingSchema _targetContextSchema;
        private readonly ConcurrentDictionary<ConnectionStringSettings, DataConnection> _connections = new ConcurrentDictionary<ConnectionStringSettings, DataConnection>();

        private bool _disposed;

        public Loader(string sourceContextName, MappingSchema sourceContextSchema, string targetContextName, MappingSchema targetContextSchema)
        {
            _sourceContextName = sourceContextName;
            _sourceContextSchema = sourceContextSchema;
            _targetContextName = targetContextName;
            _targetContextSchema = targetContextSchema;
        }

        public void Reload<T>(Func<IQuery, IEnumerable<T>> loader)
            where T : class
        {
            using (var sourceDb = CreateConnection(_sourceContextName, _sourceContextSchema))
            using (var targetDb = CreateConnection(_targetContextName, _targetContextSchema))
            {
                var query = new Query(new StubReadableDomainContextProvider((DbConnection)sourceDb.Connection, sourceDb));
                targetDb.Reload(loader(query));
            }
        }

        public void Reload<T1>(Func<IQuery, IEnumerable<T1>> loader, Expression<Func<T1, object>> key) where T1 : class
        {
            using (var sourceDb = CreateConnection(_sourceContextName, _sourceContextSchema))
            using (var targetDb = CreateConnection(_targetContextName, _targetContextSchema))
            {
                ITable<T1> temptable = null;
                var query = new Query(new StubReadableDomainContextProvider((DbConnection)sourceDb.Connection, sourceDb));
                try
                {
                    var data = loader(query);
                    var datatable = targetDb.GetTable<T1>();
                    temptable = targetDb.CreateTable<T1>($"#{Guid.NewGuid():N}");
                    temptable.BulkCopy(new BulkCopyOptions { BulkCopyTimeout = Settings.SqlBulkCopyTimeout }, data);
                    temptable.Join(datatable, key, key, (x, y) => x).Update(datatable, x => x);
                }
                finally
                {
                    temptable?.DropTable();
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var connection in _connections.Values.Select(x => x.Connection))
            {
                connection.Dispose();
            }
        }

        private DataConnection CreateConnection(string connectionStringName, MappingSchema schema)
        {
            var connectionSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionSettings == null)
            {
                throw new ArgumentException("The connection settings was not found.", "connectionStringName");
            }

            return _connections.GetOrAdd(
                connectionSettings,
                settings =>
                {
                    var provider = DataConnection.GetDataProvider(settings.Name);
                    var connection = provider.CreateConnection(settings.ConnectionString);
                    connection.Open();

                    var dataConnection = new DataConnection(provider, connection).AddMappingSchema(schema);
                    if (Settings.SqlCommandTimeout.HasValue)
                    {
                        dataConnection.CommandTimeout = Settings.SqlCommandTimeout.Value;
                    }

                    return dataConnection;
                });
        }
    }
}