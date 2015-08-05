using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IAggregateInfo : IIdentifiableInfo
    {
        IEnumerable<IValueObjectInfo> ValueObjects { get; }
    }
}