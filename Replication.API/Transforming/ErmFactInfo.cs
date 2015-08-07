using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public delegate MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> CalculateStatisticsSpecProvider(IEnumerable<long> ids);

    public abstract class ErmFactInfo
    {
        public abstract Type FactType { get; }
        public abstract IReadOnlyCollection<FactDependencyInfo> DependencyInfos { get; }
        public abstract CalculateStatisticsSpecProvider CalculateStatisticsSpecProvider { get; }
        
        public static Builder<TFact> OfType<TFact>(params object[] x) where TFact : class, IErmFactObject, IIdentifiable
        {
            return new Builder<TFact>();
        }

        public class Builder<TFact> where TFact : class, IErmFactObject, IIdentifiable
        {
            private readonly List<FactDependencyInfo> _dependencies = new List<FactDependencyInfo>();
            private MapSpecification<IQuery, IQueryable<TFact>> _mapSpec;
            private CalculateStatisticsSpecProvider _calculateStatisticsSpecProvider;

            public static implicit operator ErmFactInfo(Builder<TFact> builder)
            {
                return new ErmFactInfoImpl<TFact>(builder._dependencies, builder._mapSpec, builder._calculateStatisticsSpecProvider);
            }

            public Builder<TFact> HasSource(MapSpecification<IQuery, IQueryable<TFact>> factQueryableProvider)
            {
                _mapSpec = factQueryableProvider;
                return this;
            }

            public Builder<TFact> HasDependentAggregate<TAggregate>(Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
                where TAggregate : ICustomerIntelligenceObject
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>(dependentAggregateSpecProvider));
                return this;
            }

            public Builder<TFact> LeadsToStatisticsCalculation(CalculateStatisticsSpecProvider provider)
            {
                _calculateStatisticsSpecProvider = provider;
                return this;
            }

            public Builder<TFact> HasMatchedAggregate<TAggregate>()
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>());
                return this;
            }
        }

        public class ErmFactInfoImpl<TFact> : ErmFactInfo
            where TFact : class, IErmFactObject
        {
            private readonly IReadOnlyCollection<FactDependencyInfo> _aggregates;
            private readonly CalculateStatisticsSpecProvider _calculateStatisticsSpecProvider;

            public ErmFactInfoImpl(
                IReadOnlyCollection<FactDependencyInfo> aggregates,
                MapSpecification<IQuery, IQueryable<TFact>> mapSpecification,
                CalculateStatisticsSpecProvider calculateStatisticsSpecProvider)
            {
                _calculateStatisticsSpecProvider = calculateStatisticsSpecProvider;
                _aggregates = aggregates ?? new FactDependencyInfo[0];
                MapSpecification = mapSpecification;
                
            }

            public override Type FactType
            {
                get { return typeof(TFact); }
            }

            public override IReadOnlyCollection<FactDependencyInfo> DependencyInfos
            {
                get { return _aggregates; }
            }

            public override CalculateStatisticsSpecProvider CalculateStatisticsSpecProvider
            {
                get { return _calculateStatisticsSpecProvider; }
            }

            public MapSpecification<IQuery, IQueryable<TFact>> MapSpecification { get; private set; }
        }
    }
}