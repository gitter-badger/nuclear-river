using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IAggregateInfo : IMetadataInfo
    {
        IEnumerable<IMetadataInfo> ValueObjects { get; }
    }
}