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
        public StatisticsDependencyInfo(Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> calculateStatisticsSpecProvider)
        {
            FindSpecificationProvider = Specs.Find.ByIds<TFact>;

            CreationMappingSpecificationProvider
                = UpdatingMappingSpecificationProvider
                = DeletionMappingSpecificationProvider
                = specification => new MapSpecification<IQuery, IEnumerable>(q => calculateStatisticsSpecProvider.Invoke(specification).Map(q).Select(tuple => new CalculateStatisticsOperation { ProjectId = tuple.Item1, CategoryId = tuple.Item2 }));
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public bool IsDirectDependency
        {
            get { return false; }
        }

        public MapToObjectsSpecProvider<TFact> CreationMappingSpecificationProvider { get; private set; }

        public MapToObjectsSpecProvider<TFact> UpdatingMappingSpecificationProvider { get; private set; }

        public MapToObjectsSpecProvider<TFact> DeletionMappingSpecificationProvider { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider { get; private set; }
    }
}