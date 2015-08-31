using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.Settings
{
    public interface IReplicationSettings : ISettings
    {
        int ReplicationBatchSize { get; }
    }
}