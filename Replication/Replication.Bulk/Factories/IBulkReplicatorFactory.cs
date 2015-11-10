using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.Replicators;

namespace NuClear.Replication.Bulk.Factories
{
    public interface IBulkReplicatorFactory : IDisposable
    {
        IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement);
    }
}