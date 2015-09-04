using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public sealed class StatisticsDependencyInfo<TFact> : IFactDependencyInfo, IFactDependencyInfo<TFact>
        where TFact : class, IIdentifiable
    {
        private readonly Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> _calculateStatisticsSpecProvider;
        private readonly Func<IReadOnlyCollection<long>, FindSpecification<TFact>> _findSpecificationProvider;

        public StatisticsDependencyInfo(Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> calculateStatisticsSpecProvider)
        {
            _calculateStatisticsSpecProvider = calculateStatisticsSpecProvider;
            _findSpecificationProvider = Specs.Find.ByIds<TFact>;
        }

        public Type AggregateType
        {
            get { throw new NotImplementedException();}
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public bool IsDirectDependency
        {
            get { return false; }
        }

        public MapToObjectsSpecProvider<TFact> CreationMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _calculateStatisticsSpecProvider.Invoke(specification).Map(q).Select(tuple => new CalculateStatisticsOperation {ProjectId = tuple.Item1, CategoryId = tuple.Item2})); }
        }

        public MapToObjectsSpecProvider<TFact> UpdatingMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _calculateStatisticsSpecProvider.Invoke(specification).Map(q).Select(tuple => new CalculateStatisticsOperation { ProjectId = tuple.Item1, CategoryId = tuple.Item2 })); }
        }

        public MapToObjectsSpecProvider<TFact> DeletionMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _calculateStatisticsSpecProvider.Invoke(specification).Map(q).Select(tuple => new CalculateStatisticsOperation { ProjectId = tuple.Item1, CategoryId = tuple.Item2 })); }
        }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }
    }
}