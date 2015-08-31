using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.API.Settings
{
    public interface IReplicationSettings : ISettings
    {
        int ReplicationBatchSize { get; }
    }
}