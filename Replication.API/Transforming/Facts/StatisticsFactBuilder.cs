using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class StatisticsFactBuilder<TStatisticsFact>
        where TStatisticsFact : class
    {
        private Type _statisticsDtoType;
        private Func<long, FindSpecification<TStatisticsFact>> _findSpecificationProvider;
        private MapSpecification<IStatisticsDto, IReadOnlyCollection<TStatisticsFact>> _mapSpecification;

        public IStatisticsFactInfo Build()
        {
            return new StatisticsFactInfo<TStatisticsFact>(_statisticsDtoType, _findSpecificationProvider, _mapSpecification);
        }

        public StatisticsFactBuilder<TStatisticsFact> HasSource<TStatisticsDto>(MapSpecification<IStatisticsDto, IReadOnlyCollection<TStatisticsFact>> mapSpecification)
        {
            _statisticsDtoType = typeof(TStatisticsDto);
            _mapSpecification = mapSpecification;
            return this;
        }

        public StatisticsFactBuilder<TStatisticsFact> Aggregated(Func<long, FindSpecification<TStatisticsFact>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }
    }
}