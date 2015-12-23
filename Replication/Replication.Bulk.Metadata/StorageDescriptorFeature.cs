using LinqToDB.Mapping;

using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.Metadata
{
    public class StorageDescriptorFeature : IStorageDescriptorFeature
    {
        public StorageDescriptorFeature(ReplicationDirection direction, IConnectionStringIdentity connectionStringName, MappingSchema mappingSchema)
        {
            Direction = direction;
            ConnectionStringName = connectionStringName;
            MappingSchema = mappingSchema;
        }

        public ReplicationDirection Direction { get; }

        public IConnectionStringIdentity ConnectionStringName { get; }

        public MappingSchema MappingSchema { get; }
    }
}