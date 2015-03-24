using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    internal class DataFixtureBase
    {
        static DataFixtureBase()
        {
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1, s2);
        }

        protected static DataConnection ErmConnection
        {
            get { return CreateConnection("Erm", Schema.Erm); }
        }

        protected static DataConnection FactConnection
        {
            get { return CreateConnection("CustomerIntelligence", Schema.Fact); }
        }

        protected static DataConnection CustomerIntelligenceConnection
        {
            get { return CreateConnection("CustomerIntelligence", Schema.CustomerIntelligence); }
        }

        protected static DataConnection CreateConnection(string connectionStringName, MappingSchema schema)
        {
            var db = new DataConnection(connectionStringName);

            db.AddMappingSchema(schema);

            return db;
        }

        protected static T Read<T>(DataConnection db, long id) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(parameter, "Id"), Expression.Constant(id)), parameter);

            using (db)
            {
                return db.GetTable<T>().FirstOrDefault(predicate);
            }
        }
    }
}