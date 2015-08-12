using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public class FactInfoBuilder<TFact> where TFact : class, IErmFactObject, IIdentifiable
    {
        private readonly List<FactDependencyInfo> _dependencies = new List<FactDependencyInfo>();
        private readonly Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> _mapToTargetSpecProvider =
            ids => new MapSpecification<IQuery, IQueryable<TFact>>(q => q.For(Specs.Find.ByIds<TFact>(ids)));

        private Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> _mapToSourceSpecProvider;
        private CalculateStatisticsSpecProvider _calculateStatisticsSpecProvider;
        
        public IFactInfo Build()
        {
            return new FactInfo<TFact>(_mapToSourceSpecProvider, _mapToTargetSpecProvider, _calculateStatisticsSpecProvider, _dependencies);
        }

        public FactInfoBuilder<TFact> HasSource(Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> mapToSourceSpecProvider)
        {
            _mapToSourceSpecProvider = mapToSourceSpecProvider;
            return this;
        }

        public FactInfoBuilder<TFact> HasDependentAggregate<TAggregate>(Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
            where TAggregate : ICustomerIntelligenceObject
        {
            _dependencies.Add(FactDependencyInfo.Create<TAggregate>(dependentAggregateSpecProvider));
            return this;
        }

        public FactInfoBuilder<TFact> LeadsToStatisticsCalculation(CalculateStatisticsSpecProvider provider)
        {
            _calculateStatisticsSpecProvider = provider;
            return this;
        }

        public FactInfoBuilder<TFact> HasMatchedAggregate<TAggregate>()
        {
            _dependencies.Add(FactDependencyInfo.Create<TAggregate>());
            return this;
        }
    }
}