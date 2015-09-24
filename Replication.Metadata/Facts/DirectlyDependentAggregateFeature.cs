using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata.Facts
{
    public class DirectlyDependentAggregateFeature<T> : IDirectFactDependencyFeature, IFactDependencyFeature<T> where T : class, IIdentifiable
    {
        public DirectlyDependentAggregateFeature(Type aggregateType)
        {
            FindSpecificationProvider = Specs.Find.ByIds<T>;

            // FIXME {all, 04.09.2015}: Слабое место - внутри спецификации идентификаторы, затем идём в базу за идентификаторами. Если достать их из спецификации в бузу хдить не потребуется.
            MapSpecificationProviderOnCreate = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                                                    q => q.For(specification)
                                                                          .Select(x => x.Id)
                                                                          .Select(id => new InitializeAggregate(aggregateType, id)));
            MapSpecificationProviderOnUpdate = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                                                    q => q.For(specification)
                                                                          .Select(x => x.Id)
                                                                          .Select(id => new RecalculateAggregate(aggregateType, id)));
            MapSpecificationProviderOnDelete = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                                                    q => q.For(specification)
                                                                          .Select(x => x.Id)
                                                                          .Select(id => new DestroyAggregate(aggregateType, id)));
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