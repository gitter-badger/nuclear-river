using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IAggregateProcessor
    {
        void Initialize(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids);
        void Recalculate(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids);
        void Destroy(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids);
    }
}
