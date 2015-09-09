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
        public ValueObjectInfo(
            MapSpecification<IQuery, IQueryable<TValueObject>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> findSpecificationProvider)
        {
            FindSpecificationProvider = findSpecificationProvider;

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TValueObject>>(q => q.For<TValueObject>());
            SourceMappingProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => sourceMappingSpecification.Map(q).Where(specification));
            TargetMappingProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => targetMappingSpecification.Map(q).Where(specification));
        }

        public Type Type
        {
            get { return typeof(TValueObject); }
        }

        public MapToObjectsSpecProvider<TValueObject> SourceMappingProvider { get; private set; }

        public MapToObjectsSpecProvider<TValueObject> TargetMappingProvider { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> FindSpecificationProvider { get; private set; }
    }
}