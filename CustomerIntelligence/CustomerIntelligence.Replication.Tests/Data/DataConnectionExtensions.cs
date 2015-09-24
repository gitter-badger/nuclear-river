using System;
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
            where T : class
        {
            db.GetTable<T>().Delete();
            db.BulkCopy(new BulkCopyOptions { BulkCopyTimeout = Settings.SqlBulkCopyTimeout }, data);
        }
       
        public static void Truncate<T>(this DataConnection connection)
        {
            if (!connection.DataProvider.Name.StartsWith(ProviderName.SqlServer, StringComparison.Ordinal))
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