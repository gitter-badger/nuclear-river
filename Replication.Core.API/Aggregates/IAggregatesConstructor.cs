using System.Collections.Generic;

using NuClear.Replication.Metadata.Operations;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregatesConstructor
    {
        void Construct(IEnumerable<AggregateOperation> operations);
    }
}