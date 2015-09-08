using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class ValueObjectInfo<TValueObject> : IValueObjectInfo, IValueObjectInfo<TValueObject>
        where TValueObject : class
    {
        private readonly MapSpecification<IQuery, IQueryable<TValueObject>> _sourceMappingSpecification;
        private readonly MapSpecification<IQuery, IQueryable<TValueObject>> _targetMappingSpecification;
        private readonly Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> _findSpecificationProvider;

        public ValueObjectInfo(
            MapSpecification<IQuery, IQueryable<TValueObject>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> findSpecificationProvider)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            _targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TValueObject>>(q => q.For<TValueObject>());
            _findSpecificationProvider = findSpecificationProvider;
        }

        public Type Type
        {
            get { return typeof(TValueObject); }
        }

        public MapToObjectsSpecProvider<TValueObject> SourceMappingProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _sourceMappingSpecification.Map(q).Where(specification)); }
        }

        public MapToObjectsSpecProvider<TValueObject> TargetMappingProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _targetMappingSpecification.Map(q).Where(specification)); }
        }

        public Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }
    }
}