using System;
using System.Collections.Generic;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactDependencyInfo : IMetadataInfo
    {
        bool IsDirectDependency { get; }
    }

    public interface IFactDependencyInfo<T>
    {
        MapToObjectsSpecProvider<T, IOperation> CreationMappingSpecificationProvider { get; }
        MapToObjectsSpecProvider<T, IOperation> UpdatingMappingSpecificationProvider { get; }
        MapToObjectsSpecProvider<T, IOperation> DeletionMappingSpecificationProvider { get; }

        Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}