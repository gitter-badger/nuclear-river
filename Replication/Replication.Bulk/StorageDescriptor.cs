using LinqToDB.Mapping;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
    public sealed class StorageDescriptor
    {
        public StorageDescriptor(string connectionStringName, MappingSchema mappingSchema)
        {
            ConnectionStringName = connectionStringName;
            MappingSchema = mappingSchema;
        }

        public string ConnectionStringName { get; }
        public MappingSchema MappingSchema { get; }
    }
}