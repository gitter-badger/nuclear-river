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
        private readonly MapSpecification<IQuery, IQueryable<TFact>> _sourceMappingSpecification;
        private readonly MapSpecification<IQuery, IQueryable<TFact>> _targetMappingSpecification;
        private readonly Func<IReadOnlyCollection<long>, FindSpecification<TFact>> _findSpecificationProvider;
        private readonly IReadOnlyCollection<IFactDependencyInfo> _aggregates;

        public FactInfo(
            MapSpecification<IQuery, IQueryable<TFact>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<TFact>> findSpecificationProvider,
            IReadOnlyCollection<IFactDependencyInfo> aggregates)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            _targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TFact>>(q => q.For<TFact>());
            _findSpecificationProvider = findSpecificationProvider;
            _aggregates = aggregates ?? new IFactDependencyInfo[0];
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public IReadOnlyCollection<IFactDependencyInfo> DependencyInfos
        {
            get { return _aggregates; }
        }

        public MapToObjectsSpecProvider<TFact> SourceMappingSpecification
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _sourceMappingSpecification.Map(q).Where(specification)); }
        }

        public MapToObjectsSpecProvider<TFact> TargetMappingSpecification
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _targetMappingSpecification.Map(q).Where(specification)); }
        }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }
    }
}