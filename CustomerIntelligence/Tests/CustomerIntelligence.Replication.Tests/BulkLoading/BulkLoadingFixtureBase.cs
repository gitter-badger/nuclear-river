using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    public class BulkLoadingFixtureBase
    {
        static BulkLoadingFixtureBase()
        {
#if DEBUG
            //LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
            DataConnection.TurnTraceSwitchOn(TraceLevel.Verbose);
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1, s2);
#endif
        }

        private readonly ConcurrentDictionary<ConnectionStringSettings, DataConnection> _connections = new ConcurrentDictionary<ConnectionStringSettings, DataConnection>();

        [TearDown]
        public void FixtureTearDown()
        {
            foreach (var connection in _connections.Values.Select(x => x.Connection))
            {
                connection.Close();
            }

            _connections.Clear();
        }

        protected DataConnection CreateConnection(string connectionStringName, MappingSchema schema)
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