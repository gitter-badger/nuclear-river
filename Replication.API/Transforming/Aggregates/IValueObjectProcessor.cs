using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IValueObjectProcessor
    {
        void ApplyChanges(IReadOnlyCollection<long> ids);
    }
}
