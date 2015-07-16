using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class ErmFactInfo
    {
        public static Builder<TFact> OfType<TFact>(params object[] x) where TFact : class, IErmFactObject, IIdentifiable
        {
            return new Builder<TFact>();
        }

        public abstract Type FactType { get; }
        public abstract IReadOnlyCollection<FactDependencyInfo> DependencyInfos { get; }

        internal class Builder<TFact> where TFact : class, IErmFactObject, IIdentifiable
        {
            private readonly List<FactDependencyInfo> _dependencies = new List<FactDependencyInfo>();
            private MapSpecification<IQuery, IQueryable<TFact>> _mapSpec;

            public Builder<TFact> HasSource(MapSpecification<IQuery, IQueryable<TFact>> factQueryableProvider)
            {
                _mapSpec = factQueryableProvider;
                return this;
            }

            public Builder<TFact> HasDependentAggregate<TAggregate>(Func<IQuery, IEnumerable<long>, IEnumerable<long>> dependentAggregateIdsQueryProvider)
                where TAggregate : ICustomerIntelligenceObject
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>(dependentAggregateIdsQueryProvider));
                return this;
            }

            public Builder<TFact> HasDependentAggregate<TAggregate>(Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
                where TAggregate : ICustomerIntelligenceObject
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>(dependentAggregateSpecProvider));
                return this;
            }

            public Builder<TFact> HasMatchedAggregate<TAggregate>()
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>());
                return this;
            }

            public static implicit operator ErmFactInfo(Builder<TFact> builder)
            {
                return new ErmFactInfoImpl<TFact>(builder._dependencies, builder._mapSpec);
            }
        }

        internal class ErmFactInfoImpl<TFact> : ErmFactInfo
            where TFact : class, IErmFactObject
        {
            private readonly IReadOnlyCollection<FactDependencyInfo> _aggregates;

            public ErmFactInfoImpl(
                IReadOnlyCollection<FactDependencyInfo> aggregates,
                MapSpecification<IQuery, IQueryable<TFact>> mapSpecification)
            {
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

            public MapSpecification<IQuery, IQueryable<TFact>> MapSpecification { get; private set; }
        }
    }
}