using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class StatisticsFactInfo<T> : IStatisticsFactInfo, IStatisticsFactInfo<T>
    {
        private readonly Type _statisticsDtoType;
        private readonly Func<long, FindSpecification<T>> _findSpecificationProvider;
        private readonly MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> _mapSpecification;

        public StatisticsFactInfo(
            Type statisticsDtoType,
            Func<long, FindSpecification<T>> findSpecificationProvider, 
            MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> mapSpecification)
        {
            _statisticsDtoType = statisticsDtoType;
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public Type Type
        {
            get { return _statisticsDtoType; }
        }

        public Func<long, FindSpecification<T>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }

        public MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> MapSpecification
        {
            get { return _mapSpecification; }
        }
    }
}