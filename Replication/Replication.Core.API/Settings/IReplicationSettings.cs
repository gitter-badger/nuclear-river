using NuClear.Settings.API;

namespace NuClear.Replication.Core.API.Settings
{
    public interface IReplicationSettings : ISettings
    {
        int ReplicationBatchSize { get; }
    }
}