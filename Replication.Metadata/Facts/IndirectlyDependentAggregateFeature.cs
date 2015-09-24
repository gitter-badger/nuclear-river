using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata.Facts
{
    public class IndirectlyDependentAggregateFeature<T> : IIndirectFactDependencyFeature, IFactDependencyFeature<T> where T : IIdentifiable
    {
        public IndirectlyDependentAggregateFeature(
            Type aggregateType,
            Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<long>>> targetMappingSpecificationProvider)
        {
            FindSpecificationProvider = Specs.Find.ByIds<T>;

            MapSpecificationProviderOnCreate
                = MapSpecificationProviderOnUpdate
                  = MapSpecificationProviderOnDelete
                    = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(q => targetMappingSpecificationProvider
                                                                                                      .Invoke(specification)
                                                                                                      .Map(q)
                                                                                                      .Select(id => new RecalculateAggregate(aggregateType, id)));
        }

        public Type DependancyType
        {
            get { return typeof(T); }
        }

        public MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnCreate { get; private set; }
        public MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnUpdate { get; private set; }
        public MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnDelete { get; private set; }
        public Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; private set; }
    }
}