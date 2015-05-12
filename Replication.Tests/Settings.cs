using System.Configuration;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public static class Settings
    {
        private const int DefaultSqlBulkCopyTimeout = 30;

        public static int SqlBulkCopyTimeout
        {
            get
            {
                if (sqlBulkCopyTimeout == null)
                {
                    int timeout;
                    var timeoutValue = ConfigurationManager.AppSettings["SqlBulkCopyTimeout"];
                    if (timeoutValue != null && int.TryParse(timeoutValue, out timeout))
                    {
                        sqlBulkCopyTimeout = timeout;
                    }
                    else
                    {
                        sqlBulkCopyTimeout = DefaultSqlBulkCopyTimeout;
                    }
                }
                return sqlBulkCopyTimeout.Value;
            }
        }

        private static int? sqlBulkCopyTimeout;
    }
}