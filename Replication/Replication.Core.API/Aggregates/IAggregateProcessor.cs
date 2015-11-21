using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateProcessor
    {
        void Initialize(IReadOnlyCollection<long> ids);
        void Recalculate(IReadOnlyCollection<long> ids);
        void Destroy(IReadOnlyCollection<long> ids);
    }
}
