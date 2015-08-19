using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IAggregateInfo : IMetadataInfo
    {
        IEnumerable<IMetadataInfo> ValueObjects { get; }
    }
}