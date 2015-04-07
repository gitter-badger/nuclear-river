using System.Configuration;

using NuClear.Jobs.Settings;
using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public sealed class ReplicationServiceSettings : SettingsContainerBase,
                                                     IReplicationServiceSettings,
                                                     ITaskServiceProcessingSettings,
                                                     IPersistentStoreSettings
    {
        private const string StoreConnectionStringName = "QuartzJobStore";

        private readonly IntSetting _maxWorkingThreads = ConfigFileSetting.Int.Required("MaxWorkingThreads");
        private readonly EnumSetting<JobStoreType> _jobStoreType = ConfigFileSetting.Enum.Required<JobStoreType>("JobStoreType");
        private readonly StringSetting _schedulerName = ConfigFileSetting.String.Required("SchedulerName");
        private readonly ConnectionStringSettings _storeConnectionStringSettings;

        public ReplicationServiceSettings()
        {
            Aspects.Use(new EnvironmentsAspect());
            _storeConnectionStringSettings = ConfigurationManager.ConnectionStrings[StoreConnectionStringName];
        }

        int ITaskServiceProcessingSettings.MaxWorkingThreads
        {
            get { return _maxWorkingThreads.Value; }
        }

        JobStoreType ITaskServiceProcessingSettings.JobStoreType
        {
            get { return _jobStoreType.Value; }
        }

        string ITaskServiceProcessingSettings.SchedulerName
        {
            get { return _schedulerName.Value; }
        }

        ConnectionStringSettings IPersistentStoreSettings.ConnectionStringSettings
        {
            get { return _storeConnectionStringSettings; }
        }
    }
}