using System.Configuration;

using NuClear.AdvancedSearch.Settings;
using NuClear.Jobs.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public class PersistentStoreAspect : ISettingsAspect, IPersistentStoreSettings
    {
        private readonly ConnectionStringSettings _storeConnectionStringSettings;

        public PersistentStoreAspect(IConnectionStringSettings connectionStringSettings)
        {
            _storeConnectionStringSettings = connectionStringSettings.GetConnectionStringSettings(ConnectionStringName.QuartzJobStore);
        }

        ConnectionStringSettings IPersistentStoreSettings.ConnectionStringSettings
        {
            get { return _storeConnectionStringSettings; }
        }
    }
}