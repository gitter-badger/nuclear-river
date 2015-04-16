using System.Configuration;

using NuClear.Jobs.Settings;
using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public sealed class ReplicationServiceSettings : SettingsContainerBase,
                                                     IReplicationServiceSettings,
                                                     ITaskServiceProcessingSettings
    {

        private readonly IntSetting _maxWorkingThreads = ConfigFileSetting.Int.Required("MaxWorkingThreads");
        private readonly EnumSetting<JobStoreType> _jobStoreType = ConfigFileSetting.Enum.Required<JobStoreType>("JobStoreType");
        private readonly StringSetting _schedulerName = ConfigFileSetting.String.Required("SchedulerName");

        public ReplicationServiceSettings()
        {
            var serviceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;

            Aspects.Use(new EnvironmentsAspect());
            Aspects.Use(new PersistentStoreAspect());
            Aspects.Use(new ServiceBusReceiverSettingsAspect(serviceBusConnectionString));
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
    }
}