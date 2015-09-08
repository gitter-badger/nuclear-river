using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class AggregateInfo<T> : IAggregateInfo, IAggregateInfo<T>
        where T : class, IIdentifiable
    {
        private readonly MapSpecification<IQuery, IQueryable<T>> _sourceMappingSpecification;
        private readonly MapSpecification<IQuery, IQueryable<T>> _targetMappingSpecification;
        private readonly Func<IReadOnlyCollection<long>, FindSpecification<T>> _findSpecificationProvider;

        public AggregateInfo(
            MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<T>> findSpecificationProvider,
            IReadOnlyCollection<IValueObjectInfo> valueObjects = null)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            _targetMappingSpecification = new MapSpecification<IQuery, IQueryable<T>>(q => q.For<T>());
            _findSpecificationProvider = findSpecificationProvider;
            ValueObjects = valueObjects;
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public MapToObjectsSpecProvider<T> SourceMappingProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _sourceMappingSpecification.Map(q).Where(specification)); }
        }

        public MapToObjectsSpecProvider<T> TargetMappingProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _targetMappingSpecification.Map(q).Where(specification)); }
        }

        public Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }

        public IReadOnlyCollection<IValueObjectInfo> ValueObjects { get; private set; }
    }
}