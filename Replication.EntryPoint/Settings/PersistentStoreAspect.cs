using System.Configuration;

using NuClear.AdvancedSearch.Common.Settings;
using NuClear.Jobs.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public class PersistentStoreAspect : ISettingsAspect, IPersistentStoreSettings
    {
        private readonly ConnectionStringSettings _storeConnectionStringSettings;

        public PersistentStoreAspect(IConnectionStringSettings connectionStringSettings)
        {
            _storeConnectionStringSettings = connectionStringSettings.GetConnectionStringSettings(ConnectionStringName.Infrastructure);
        }

        string IPersistentStoreSettings.ConnectionString
        {
            get { return _storeConnectionStringSettings.ConnectionString; }
        }

        public string TablePrefix { get { return "Quartz."; } }
    }
}