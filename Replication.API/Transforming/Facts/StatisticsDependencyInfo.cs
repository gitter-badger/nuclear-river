using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    // TODO {all, 15.09.2015}: Подумать о правильном поядке вызова при создании/обновлении/удалении факта (до/после - аналогично AggregateDependencyInfo или должен отличаться?)
    public sealed class StatisticsDependencyInfo<TFact> : IFactDependencyInfo, IFactDependencyInfo<TFact>
        where TFact : class, IIdentifiable
    {
        public StatisticsDependencyInfo(Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> calculateStatisticsSpecProvider)
        {
            FindSpecificationProvider = Specs.Find.ByIds<TFact>;

            MapSpecificationProviderOnCreate
                = MapSpecificationProviderOnUpdate
                = MapSpecificationProviderOnDelete
                = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(q => calculateStatisticsSpecProvider.Invoke(specification).Map(q).Select(tuple => new CalculateStatisticsOperation { ProjectId = tuple.Item1, CategoryId = tuple.Item2 }));
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public bool IsDirectDependency
        {
            get { return false; }
        }

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnCreate { get; private set; }

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnUpdate { get; private set; }

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnDelete { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider { get; private set; }
    }
}