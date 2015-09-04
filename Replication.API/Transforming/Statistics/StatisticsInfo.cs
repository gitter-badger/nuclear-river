using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsInfo<T> : IStatisticsInfo, IStatisticsInfo<T>
    {
        private readonly MapSpecification<IQuery, IQueryable<T>> _sourceMappingSpecification;
        private readonly MapSpecification<IQuery, IQueryable<T>> _targetMappingSpecification;
        private readonly Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> _findSpecificationProvider;

        public StatisticsInfo(
            MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification,
            MapSpecification<IQuery, IQueryable<T>> targetMappingSpecification,
            Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            _sourceMappingSpecification = sourceMappingSpecification;
            _targetMappingSpecification = targetMappingSpecification;
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public MapToObjectsSpecProvider<T, T> SourceMappingSpecification
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable<T>>(q => _sourceMappingSpecification.Map(q).Where(specification)); }
        }

        public MapToObjectsSpecProvider<T, T> TargetMappingSpecification
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable<T>>(q => _targetMappingSpecification.Map(q).Where(specification)); }
        }

        public Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; } 
        }
    }
}