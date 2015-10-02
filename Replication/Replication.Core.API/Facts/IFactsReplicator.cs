using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactsReplicator
    {
        IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations, IComparer<Type> factTypePriorityComparer);
    }
}