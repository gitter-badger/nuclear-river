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
            MapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<TValueObject>>(q => sourceMappingSpecification.Map(q).Where(specification));
            MapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<TValueObject>>(q => targetMappingSpecification.Map(q).Where(specification));
        }

        public Type Type
        {
            get { return typeof(TValueObject); }
        }

        public MapToObjectsSpecProvider<TValueObject, TValueObject> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<TValueObject, TValueObject> MapSpecificationProviderForTarget { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> FindSpecificationProvider { get; private set; }
    }
}