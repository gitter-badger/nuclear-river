using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.Api.Replicators;

namespace NuClear.Replication.Bulk.Api.Factories
{
    public interface IBulkReplicatorFactory : IDisposable
    {
        IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement);
    }
}