using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public delegate MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> CalculateStatisticsSpecProvider(IReadOnlyCollection<long> ids);

    public interface IFactInfo : IMetadataInfo
    {
        IReadOnlyCollection<IFactDependencyInfo> DependencyInfos { get; }
        CalculateStatisticsSpecProvider CalculateStatisticsSpecProvider { get; } 
    }
}