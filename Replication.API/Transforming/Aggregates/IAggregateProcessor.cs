using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IAggregateProcessor
    {
        void Initialize(IReadOnlyCollection<long> ids);
        void Recalculate(IReadOnlyCollection<long> ids);
        void Destroy(IReadOnlyCollection<long> ids);
    }
}
