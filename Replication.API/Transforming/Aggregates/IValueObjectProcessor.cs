using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IValueObjectProcessor
    {
        void ApplyChanges(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids);
    }
}
