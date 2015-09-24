using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata.Facts
{
    // TODO {all, 15.09.2015}: Подумать о правильном поядке вызова при создании/обновлении/удалении факта (до/после - аналогично *DependentAggregateFeature или должен отличаться?)
    public class DependentStatisticsFeature<T> : IFactDependencyFeature<T> where T : IIdentifiable
    {
        public DependentStatisticsFeature(Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> calculateStatisticsSpecProvider)
        {
            FindSpecificationProvider = Specs.Find.ByIds<T>;

            MapSpecificationProviderOnCreate
                = MapSpecificationProviderOnUpdate
                  = MapSpecificationProviderOnDelete
                    = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                           q => calculateStatisticsSpecProvider.Invoke(specification)
                                                                               .Map(q)
                                                                               .Select(tuple => new RecalculateStatisticsOperation
                                                                                                {
                                                                                                    ProjectId = tuple.Item1,
                                                                                                    CategoryId = tuple.Item2
                                                                                                }));
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