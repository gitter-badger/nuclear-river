using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
    internal class ViewContainer : IDisposable
    {
        private readonly string _connectionString;
        private readonly List<StringCollection> _views;
        private bool _disposed;

        private ViewContainer(string connectionString, List<StringCollection> views)
        {
            _connectionString = connectionString;
            _views = views;
        }

        public static IDisposable TemporaryRemoveViews(Storage storage)
        {
            var connectionString = GetConnectionString(storage);
            var database = GetDatabase(connectionString);
            var views = new List<StringCollection>();
            foreach (var view in database.Views.Cast<View>().Where(v => !v.IsSystemObject).ToArray())
            {
                views.Add(view.Script());
                view.Drop();
            }

            return new ViewContainer(connectionString, views);
        }

        private static string GetConnectionString(Storage storage)
        {
            return ConfigurationManager.ConnectionStrings[storage.ConnectionStringName].ConnectionString;
        }

        private static Database GetDatabase(string connectionString)
        {
            var connection = new ServerConnection { ConnectionString = connectionString };
            var server = new Server(connection);

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            return server.Databases[connectionStringBuilder.InitialCatalog];
        }

        ~ViewContainer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();

                    foreach (var view in _views)
                    {
                        foreach (var s in view)
                        {
                            var command = new SqlCommand(s, sqlConnection);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}