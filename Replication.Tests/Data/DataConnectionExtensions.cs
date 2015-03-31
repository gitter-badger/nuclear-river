using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    internal static class DataConnectionExtensions
    {
        public static T Read<T>(this DataConnection db, long id) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(parameter, "Id"), Expression.Constant(id)), parameter);

            return db.GetTable<T>().FirstOrDefault(predicate);
        }

        public static void Reload<T>(this DataConnection db, IEnumerable<T> data)
        {
            db.Truncate<T>();
            db.BulkCopy(data);
        }
       
        public static void Truncate<T>(this DataConnection connection)
        {
            if (connection.DataProvider.Name != ProviderName.SqlServer)
            {
                return;
            }

            var annotation = connection.MappingSchema.GetAttributes<TableAttribute>(typeof(T)).FirstOrDefault();
            if (annotation != null)
            {
                connection.Execute(string.Format("truncate table [{0}].[{1}]", annotation.Schema, annotation.Name ?? typeof(T).Name));
            }
        }
    }
}