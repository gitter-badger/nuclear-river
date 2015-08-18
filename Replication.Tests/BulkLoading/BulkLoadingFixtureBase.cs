using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Expressions;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.BulkLoading
{
    public class BulkLoadingFixtureBase
    {
        private static readonly MethodInfo CreateTableMethodInfo =
            MemberHelper.MethodOf(() => default(IDataContext).CreateTable<object>(default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  DefaulNullable.None))
                        .GetGenericMethodDefinition();

        private static readonly Lazy<Type[]> Tables = new Lazy<Type[]>(
            () =>
            {
                var accessor = typeof(Firm);
                return accessor.Assembly.GetTypes()
                               .Where(t => t.IsClass && (t.Namespace ?? string.Empty).Contains(accessor.Namespace ?? string.Empty))
                               .ToArray();
            });

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

                    return TuneConnectionIfSqlite(dataConnection);
                });
        }

        private static DataConnection TuneConnectionIfSqlite(DataConnection db)
        {
            if (db.DataProvider.Name == ProviderName.SQLite)
            {
                using (new NoSqlTrace())
                {
                    var schema = db.MappingSchema;
                    foreach (var table in Tables.Value)
                    {
                        var attributes = schema.GetAttributes<TableAttribute>(table);
                        if (attributes != null && attributes.Length > 0)
                        {
                            // SQLite does not support schemas
                            Array.ForEach(attributes, attr => attr.Schema = null);

                            // create empty table
                            CreateTableMethodInfo.MakeGenericMethod(table).Invoke(null, new object[] { db, null, null, null, null, null, DefaulNullable.None });
                        }
                    }

                    /*
                    // SQLite does not support schemas
                    Tables.Value.SelectMany(table => db.MappingSchema.GetAttributes<TableAttribute>(table)).ToList().ForEach(x => x.Schema = null);
                     */
                }
            }

            return db;
        }
    }
}