using System;

using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.Metadata
{
    public class ConfigStorageDescriptorFeature : IStorageDescriptorFeature
    {
        public ConfigStorageDescriptorFeature(ReplicationDirection direction, IConnectionStringIdentity path, Type parserType)
        {
            Direction = direction;
            PathIdentity = path;
            ParcerType = parserType;
        }

        public IConnectionStringIdentity PathIdentity { get; }

        public ReplicationDirection Direction { get; }

        public Type ParcerType { get; }
    }
}