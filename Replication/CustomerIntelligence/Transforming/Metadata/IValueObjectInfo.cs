using System.Collections;
using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IValueObjectInfo : IMetadataInfo
    {
        IEnumerable QueryByParentIds(IQuery context, IReadOnlyCollection<long> parentIds);
    }
}