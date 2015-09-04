using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public class AggregateInfoBuilder<TAggregate> where TAggregate : class, ICustomerIntelligenceObject, IIdentifiable
    {
        private readonly List<IValueObjectInfo> _valueObjects;

        private MapSpecification<IQuery, IQueryable<TAggregate>> _mapToSourceSpec;

        public AggregateInfoBuilder()
        {
            _valueObjects = new List<IValueObjectInfo>();
        }

        public IAggregateInfo Build()
        {
            return new AggregateInfo<TAggregate>(_mapToSourceSpec, Specs.Find.ByIds<TAggregate>, _valueObjects);
        }

        public AggregateInfoBuilder<TAggregate> HasSource(MapSpecification<IQuery, IQueryable<TAggregate>> mapToSourceSpec)
        {
            _mapToSourceSpec = mapToSourceSpec;
            return this;
        }

        public AggregateInfoBuilder<TAggregate> HasValueObject<TValueObject>(
            MapSpecification<IQuery, IQueryable<TValueObject>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> findSpecificationProvider) 
            where TValueObject : class
        {
            _valueObjects.Add(new ValueObjectInfo<TValueObject>(sourceMappingSpecification, findSpecificationProvider));
            return this;
        }
    }
}