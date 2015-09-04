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
    public sealed class AggregateDependencyInfo<TFact> : IFactDependencyInfo, IFactDependencyInfo<TFact> 
        where TFact : class, IIdentifiable
    {
        private readonly Type _aggregateType;
        private readonly Func<IReadOnlyCollection<long>, FindSpecification<TFact>> _findSpecificationProvider;
        private readonly Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<long>>> _targetMappingSpecificationProvider;

        public AggregateDependencyInfo(Type aggregateType,
                                       Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<long>>> targetMappingSpecificationProvider)
        {
            _aggregateType = aggregateType;
            _findSpecificationProvider = Specs.Find.ByIds<TFact>;
            _targetMappingSpecificationProvider = targetMappingSpecificationProvider;
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
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _targetMappingSpecificationProvider.Invoke(specification).Map(q).Select(id => new RecalculateAggregate(_aggregateType, id))); }
        }

        public MapToObjectsSpecProvider<TFact> UpdatingMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _targetMappingSpecificationProvider.Invoke(specification).Map(q).Select(id => new RecalculateAggregate(_aggregateType, id))); }
        }

        public MapToObjectsSpecProvider<TFact> DeletionMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => _targetMappingSpecificationProvider.Invoke(specification).Map(q).Select(id => new RecalculateAggregate(_aggregateType, id))); }
        }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }
    }
}