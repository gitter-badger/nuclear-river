using System;
using System.Collections.Generic;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactInfo : IMetadataInfo
    {
        IReadOnlyCollection<IFactDependencyInfo> DependencyInfos { get; }
    }

    public interface IFactInfo<T>
    {
        MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; }
        MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; }

        Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}