using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsInfoBuilder<T>
    {
        private MapSpecification<IQuery, IQueryable<T>> _sourceMappingSpecification;
        private MapSpecification<IQuery, IQueryable<T>> _targetMappingSpecification;
        private Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> _findSpecificationProvider;
        private IEqualityComparer<T> _comparer;

        public IStatisticsInfo Build()
        {
            return new StatisticsInfo<T>(_sourceMappingSpecification, _targetMappingSpecification, _findSpecificationProvider, _comparer);
        }

        public StatisticsInfoBuilder<T> HasSource(MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            return this;
        }

        public StatisticsInfoBuilder<T> HasTarget(MapSpecification<IQuery, IQueryable<T>> targetMappingSpecification)
        {
            _targetMappingSpecification = targetMappingSpecification;
            return this;
        }

        public StatisticsInfoBuilder<T> HasFilter(Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }

        public StatisticsInfoBuilder<T> HasFieldComparer(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
            return this;
        }
    }
}