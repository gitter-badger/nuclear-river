using System.Configuration;

using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public interface IConnectionStringSettings : ISettings
    {
        ConnectionStringSettings GetConnectionStringSettings(string connectionStringName);
    }

    public sealed class ConnectionStringsSettingsAspect : ISettingsAspect, IConnectionStringSettings
    {
        public ConnectionStringSettings GetConnectionStringSettings(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName];
        }
    }
}