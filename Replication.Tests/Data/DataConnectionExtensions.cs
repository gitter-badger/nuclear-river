using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    internal static class DataConnectionExtensions
    {
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