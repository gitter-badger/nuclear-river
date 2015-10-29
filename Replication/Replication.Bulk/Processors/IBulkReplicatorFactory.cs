using System.Collections.Generic;

using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
    public interface IBulkReplicatorFactory
    {
        IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement);
    }
}