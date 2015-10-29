using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public class StorageDescriptorFeature : IMetadataFeature
    {
        
        public StorageDescriptorFeature(ReplicationDirection direction, string connectionStringName, MappingSchema mappingSchema)
        {
            Direction = direction;
            ConnectionStringName = connectionStringName;
            MappingSchema = mappingSchema;
        }

        public ReplicationDirection Direction { get; private set; }
        
        public string ConnectionStringName { get; private set; }

        public MappingSchema MappingSchema { get; private set; }
    }
}