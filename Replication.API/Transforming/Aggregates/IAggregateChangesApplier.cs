using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IAggregateChangesApplier
    {
        void Create<T>(IReadOnlyCollection<T> source);
    }

    public interface IAggregateChangesApplier<TAggregate> where TAggregate : class, IIdentifiable
    {
    }
}