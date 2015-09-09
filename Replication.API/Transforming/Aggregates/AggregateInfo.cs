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
        public AggregateInfo(
            MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<T>> findSpecificationProvider,
            IReadOnlyCollection<IValueObjectInfo> valueObjects = null)
        {
            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<T>>(q => q.For<T>());

            FindSpecificationProvider = findSpecificationProvider;
            ValueObjects = valueObjects;
            SourceMappingProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => sourceMappingSpecification.Map(q).Where(specification));
            TargetMappingProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => targetMappingSpecification.Map(q).Where(specification));
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public MapToObjectsSpecProvider<T> SourceMappingProvider { get; private set; }

        public MapToObjectsSpecProvider<T> TargetMappingProvider { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; private set; }

        public IReadOnlyCollection<IValueObjectInfo> ValueObjects { get; private set; }
    }
}