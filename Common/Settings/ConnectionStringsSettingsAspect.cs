using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Common.Settings
{
    [Obsolete("Use NuClear.Storage.ConnectionStrings.ConnectionStringSettingsAspect")]
    public sealed class ConnectionStringsSettingsAspect : ISettingsAspect, IConnectionStringSettings
    {
        private readonly IReadOnlyDictionary<ConnectionStringName, ConnectionStringSettings> _connectionStringsMap;

        public ConnectionStringsSettingsAspect()
        {
            var specifiedConnectionStringsMap = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().ToDictionary(connection => connection.Name);

            var availableConnectionStringsMap = new Dictionary<ConnectionStringName, ConnectionStringSettings>();
            foreach (var connectionStringAlias in Enum.GetValues(typeof(ConnectionStringName)).Cast<ConnectionStringName>())
            {
                var connectionStringName = connectionStringAlias.ToString();
                ConnectionStringSettings connection;
                if (specifiedConnectionStringsMap.TryGetValue(connectionStringName, out connection))
                {
                    availableConnectionStringsMap[connectionStringAlias] = connection;
                }
            }

            _connectionStringsMap = availableConnectionStringsMap;
        }

        public string GetConnectionString(ConnectionStringName connectionStringNameAlias)
        {
            return GetConnectionStringSettings(connectionStringNameAlias).ConnectionString;
        }

        public ConnectionStringSettings GetConnectionStringSettings(ConnectionStringName connectionStringNameAlias)
        {
            ConnectionStringSettings connectionStringSettings;
            if (!_connectionStringsMap.TryGetValue(connectionStringNameAlias, out connectionStringSettings))
            {
                throw new ConfigurationErrorsException(string.Format("Can't find connection string for alias '{0}'", connectionStringNameAlias));
            }

            return connectionStringSettings;
        }
    }
}
