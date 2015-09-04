using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class FactInfoBuilder<TFact> where TFact : class, IErmFactObject, IIdentifiable
    {
        private readonly List<IFactDependencyInfo> _dependencies = new List<IFactDependencyInfo>();

        private MapSpecification<IQuery, IQueryable<TFact>> _sourceMappingSpecification;
        
        public IFactInfo Build()
        {
            return new FactInfo<TFact>(_sourceMappingSpecification, Specs.Find.ByIds<TFact>, _dependencies);
        }

        public FactInfoBuilder<TFact> HasSource(MapSpecification<IQuery, IQueryable<TFact>> sourceMappingSpecification)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            return this;
        }

        public FactInfoBuilder<TFact> HasDependentAggregate<TAggregate>(Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
            where TAggregate : class, IIdentifiable
        {
            // FIXME {all, 03.09.2015}: TAggregate заменить на идентификатор типа
            var dependency = new AggregateDependencyInfo<TFact>(typeof(TAggregate), dependentAggregateSpecProvider);
            _dependencies.Add(dependency);
            return this;
        }

        public FactInfoBuilder<TFact> HasMatchedAggregate<TAggregate>() 
            where TAggregate : class, IIdentifiable
        {
            // FIXME {all, 03.09.2015}: TAggregate заменить на идентификатор типа
            var dependency = new DirectAggregateDependencyInfo<TFact>(typeof(TAggregate));
            _dependencies.Add(dependency); 
            return this;
        }

        public FactInfoBuilder<TFact> LeadsToStatisticsCalculation(Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> provider)
        {
            var dependency = new StatisticsDependencyInfo<TFact>(provider);
            _dependencies.Add(dependency);
            return this;
        }
    }
}