using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IAggregateInfo : IMetadataInfo
    {
        IEnumerable<IMetadataInfo> ValueObjects { get; }
    }
}