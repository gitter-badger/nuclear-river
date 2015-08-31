using NuClear.AdvancedSearch.Replication.API.Settings;
using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public sealed class ReplicationSettingsAspect : ISettingsAspect, IReplicationSettings
    {
        private readonly IntSetting _sqlCommandTimeout = ConfigFileSetting.Int.Required("ReplicationBatchSize");

        public int ReplicationBatchSize
        {
            get { return _sqlCommandTimeout.Value; }
        }
    }
}
