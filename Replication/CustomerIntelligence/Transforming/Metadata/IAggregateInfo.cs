using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IAggregateInfo : IIdentifiableInfo
    {
        IEnumerable<IEntityInfo> Entities { get; }

        IEnumerable<IValueObjectInfo> ValueObjects { get; }
    }
}