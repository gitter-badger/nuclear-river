using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.Replication.Bulk.Metadata
{
    public interface IStorageDescriptorFeature : IMetadataFeature
    {
        ReplicationDirection Direction { get; }
    }
}