using System;
using System.Collections.Generic;

using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactsReplicator
    {
        IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations, IComparer<Type> factTypePriorityComparer);
    }
}