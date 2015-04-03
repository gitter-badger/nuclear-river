using NuClear.Settings.API;

namespace Replication.EntryPoint.Settings
{
    public sealed class ReplicationServiceSettings : SettingsContainerBase,
                                                     IReplicationServiceSettings
    {
        public ReplicationServiceSettings()
        {
            Aspects.Use(new EnvironmentsAspect());
        }
    }
}