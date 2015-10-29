using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.References;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public sealed class BulkReplicationMetadataBuilder : MetadataElementBuilder<BulkReplicationMetadataBuilder, BulkReplicationMetadataElement>
    {
        public BulkReplicationMetadataBuilder DefinesReplication => this;

        public BulkReplicationMetadataBuilder CommandlineKey(string key)
        {
            AddFeatures(new CommandLineFeature(key));
            return this;
        }

        public BulkReplicationMetadataBuilder From(string connectionStringName, MappingSchema mappingSchema)
        {
            AddFeatures(new StorageDescriptorFeature(ReplicationDirection.From,  connectionStringName, mappingSchema));
            return this;
        }

        public BulkReplicationMetadataBuilder To(string connectionStringName, MappingSchema mappingSchema)
        {
            AddFeatures(new StorageDescriptorFeature(ReplicationDirection.To, connectionStringName, mappingSchema));
            return this;
        }
        
        public BulkReplicationMetadataBuilder UsingMetadataOfKind<TIdentity>(params string[] segments)
            where TIdentity : MetadataKindIdentityBase<TIdentity>, new()
        {
            var referencedElementId = Metadata.Id.For<TIdentity>().Segments(segments);
            Childs(MetadataReference.Config.For(referencedElementId));
            return this;
        }

        public BulkReplicationMetadataBuilder EssentialView(string viewName)
        {
            AddFeatures(new EssentialViewFeature(viewName));
            return this;
        }

        protected override BulkReplicationMetadataElement Create()
        {
            return new BulkReplicationMetadataElement(Features);
        }
    }
}