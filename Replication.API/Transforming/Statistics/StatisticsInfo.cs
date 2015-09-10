using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsInfo<T> : IStatisticsInfo, IStatisticsInfo<T>
    {
        public StatisticsInfo(
            MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification,
            MapSpecification<IQuery, IQueryable<T>> targetMappingSpecification,
            Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> findSpecificationProvider,
            IEqualityComparer<T> fieldComparer)
        {
            FieldComparer = fieldComparer;
            FindSpecificationProvider = findSpecificationProvider;

            MapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<T>>(q => sourceMappingSpecification.Map(q).Where(specification));
            MapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification));
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; private set; }

        public Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> FindSpecificationProvider { get; private set; }

        public IEqualityComparer<T> FieldComparer { get; private set; }
    }
}