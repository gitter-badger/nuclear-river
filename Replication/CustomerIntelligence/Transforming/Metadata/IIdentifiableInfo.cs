using System.Collections;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IIdentifiableInfo : IMetadataInfo
    {
        MapSpecification<IQuery, IEnumerable> GetMapToSourceSpec(IReadOnlyCollection<long> ids);
        MapSpecification<IQuery, IEnumerable<long>> GetMapToSourceIdsSpec(IReadOnlyCollection<long> ids);
        MapSpecification<IQuery, IEnumerable> GetMapToTargetSpec(IReadOnlyCollection<long> ids);
        MapSpecification<IQuery, IEnumerable<long>> GetMapToTargetIdsSpec(IReadOnlyCollection<long> ids);
    }
}