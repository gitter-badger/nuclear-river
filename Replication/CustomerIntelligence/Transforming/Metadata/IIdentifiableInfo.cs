using System.Collections;
using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IIdentifiableInfo : IMetadataInfo
    {
        IEnumerable QueryByIds(IQuery query, IReadOnlyCollection<long> ids);
        IEnumerable<long> QueryIdsByIds(IQuery query, IReadOnlyCollection<long> ids);
    }
}