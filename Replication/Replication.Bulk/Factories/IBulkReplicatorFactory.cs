using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.Bulk.Replicators;
using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.Replication.Bulk.Factories
{
    public interface IBulkReplicatorFactory : IDisposable
    {
        IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement);
    }
}