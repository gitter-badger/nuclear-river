using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class FactInfo<TFact> : IFactInfo, IFactInfo<TFact>
        where TFact : class, IErmFactObject
    {
        public FactInfo(
            MapSpecification<IQuery, IQueryable<TFact>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<TFact>> findSpecificationProvider,
            IReadOnlyCollection<IFactDependencyInfo> aggregates)
        {
            DependencyInfos = aggregates ?? new IFactDependencyInfo[0];
            FindSpecificationProvider = findSpecificationProvider;

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TFact>>(q => q.For<TFact>());
            SourceMappingProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => sourceMappingSpecification.Map(q).Where(specification));
            TargetMappingProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => targetMappingSpecification.Map(q).Where(specification));
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public IReadOnlyCollection<IFactDependencyInfo> DependencyInfos { get; private set; }

        public MapToObjectsSpecProvider<TFact> SourceMappingProvider { get; private set; }

        public MapToObjectsSpecProvider<TFact> TargetMappingProvider { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider { get; private set; }
    }
}