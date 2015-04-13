using System.Configuration;

using NuClear.Jobs.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public class PersistentStoreAspect : ISettingsAspect, IPersistentStoreSettings
    {
        private const string StoreConnectionStringName = "QuartzJobStore";

        private readonly ConnectionStringSettings _storeConnectionStringSettings;

        public PersistentStoreAspect()
        {
            _storeConnectionStringSettings = ConfigurationManager.ConnectionStrings[StoreConnectionStringName];
        }


        ConnectionStringSettings IPersistentStoreSettings.ConnectionStringSettings
        {
            get { return _storeConnectionStringSettings; }
        }
    }
}