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

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    internal abstract class BaseDataFixture : BaseFixture
    {
        private static readonly ConcurrentDictionary<ConnectionStringSettings, DataConnection> Connections = new ConcurrentDictionary<ConnectionStringSettings, DataConnection>();

        static BaseDataFixture()
        {
#if DEBUG
            //LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
            DataConnection.TurnTraceSwitchOn(TraceLevel.Verbose);
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1, s2);
#endif
        }

        [TearDown]
        public void FixtureTearDown()
        {
            foreach (var connection in Connections.Values.Select(x => x.Connection))
            {
                connection.Close();
            }
            Connections.Clear();
        }

        protected static IQueryable<T> Inquire<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }

        protected DataConnection ErmDb
        {
            get { return CreateConnection("Erm", Schema.Erm); }
        }

        protected DataConnection FactsDb
        {
            get { return CreateConnection("Facts", Schema.Facts); }
        }

        protected DataConnection CustomerIntelligenceDb
        {
            get { return CreateConnection("CustomerIntelligence", Schema.CustomerIntelligence); }
        }

        protected DataConnection CreateConnection(string connectionStringName, MappingSchema schema)
        {
            var connectionSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionSettings == null)
            {
                throw new ArgumentException("The connection settings was not found.", "connectionStringName");
            }

            return Connections.GetOrAdd(
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

                    // SQLite does not support schemas
                    Tables.Value.SelectMany(table => db.MappingSchema.GetAttributes<TableAttribute>(table)).ToList().ForEach(x => x.Schema = null);
                }
            }
            return db;
        }

        private static readonly MethodInfo CreateTableMethodInfo = MemberHelper.MethodOf(() => DataExtensions.CreateTable<object>(default(IDataContext), default(string), default(string), default(string), default(string), default(string), DefaulNullable.None)).GetGenericMethodDefinition();

        private static readonly Lazy<Type[]> Tables = new Lazy<Type[]>(
            () =>
            {
                var accessor = typeof(Firm);
                return accessor.Assembly.GetTypes()
                    .Where(t => t.IsClass && (t.Namespace ?? "").Contains(accessor.Namespace ?? ""))
                    .ToArray();
            });
    }
}