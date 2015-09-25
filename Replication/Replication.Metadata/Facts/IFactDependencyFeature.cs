using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Replication.Metadata.Model;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata.Facts
{
    public interface IFactDependencyFeature : IMetadataFeature
    {
        Type DependancyType { get; }
    }

    public interface IIndirectFactDependencyFeature : IFactDependencyFeature
    {
    }
    
    public interface IFactDependencyFeature<T> : IFactDependencyFeature
    {
        MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnCreate { get; }
        MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnUpdate { get; }
        MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnDelete { get; }

        Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}