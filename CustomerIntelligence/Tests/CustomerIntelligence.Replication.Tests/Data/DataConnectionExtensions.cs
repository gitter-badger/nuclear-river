using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;
using LinqToDB.Data;

namespace NuClear.CustomerIntelligence.Replication.Tests.Data
{
    internal static class DataConnectionExtensions
    {
        public static void Reload<T>(this DataConnection db, IEnumerable<T> data)
            where T : class
        {
            db.GetTable<T>().Delete();
            db.BulkCopy(new BulkCopyOptions { BulkCopyTimeout = Settings.SqlBulkCopyTimeout }, data);
        }

        public static void Reload<T, TKey>(this DataConnection db, IEnumerable<T> data, Expression<Func<T, TKey>> keyExpression)
            where T : class
        {
            ITable<T> temptable = null;
            try
            {
                var datatable = db.GetTable<T>();
                temptable = db.CreateTable<T>($"#{Guid.NewGuid():N}");
                temptable.BulkCopy(new BulkCopyOptions { BulkCopyTimeout = Settings.SqlBulkCopyTimeout }, data);
                temptable.Join(datatable, keyExpression, keyExpression, (x, y) => x).Update(datatable, x => x);
            }
            finally
            {
                temptable?.DropTable();
            }
        }
    }
}