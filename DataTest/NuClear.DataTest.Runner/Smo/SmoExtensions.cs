using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace NuClear.DataTest.Runner.Smo
{
    internal static class SmoExtensions
    {
        public static Database GetDatabase(this string connectionString)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            var databaseName = (string)builder["Initial Catalog"];
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            var sqlConnection = new SqlConnection(builder.ConnectionString);
            var serverConnection = new ServerConnection(sqlConnection);
            var server = new Server(serverConnection);

            var database = server.Databases[databaseName];
            if (database == null)
            {
                throw new ArgumentException(nameof(connectionString));
            }

            return database;
        }

        public static Database PrefetchTablesAndViews(this Database database)
        {
            database.PrefetchObjects(typeof(Table));
            database.PrefetchObjects(typeof(View));

            return database;
        }

        public static IEnumerable<TableViewTableTypeBase> GetTablesAndViewsFor(this Database database, string schema)
        {
            var tables = database.Tables.Cast<Table>().Where(x => !x.IsSystemObject).Where(x => x.Schema == schema);
            var views = database.Views.Cast<View>().Where(x => !x.IsSystemObject).Where(x => x.Schema == schema);

            return tables.Concat(views.Cast<TableViewTableTypeBase>());
        }
    }
}