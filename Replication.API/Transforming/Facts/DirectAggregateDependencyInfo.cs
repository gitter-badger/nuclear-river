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
    public sealed class DirectAggregateDependencyInfo<TFact> : IFactDependencyInfo, IFactDependencyInfo<TFact>
        where TFact : class, IIdentifiable
    {
        private readonly Type _aggregateType;
        private readonly Func<IReadOnlyCollection<long>, FindSpecification<TFact>> _findSpecificationProvider;

        public DirectAggregateDependencyInfo(Type aggregateType)
        {
            _aggregateType = aggregateType;
            _findSpecificationProvider = Specs.Find.ByIds<TFact>;
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public bool IsDirectDependency
        {
            get { return true; }
        }

        public MapToObjectsSpecProvider<TFact> CreationMappingSpecificationProvider
        {
            // FIXME {all, 04.09.2015}: Слабое место - внутри спецификации идентификаторы, затем идём в базу за идентификаторами. Если достать их из спецификации в бузу хдить не потребуется.
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => q.For(specification).Select(x => x.Id).Select(id => new InitializeAggregate(_aggregateType, id))); }
        }

        public MapToObjectsSpecProvider<TFact> UpdatingMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => q.For(specification).Select(x => x.Id).Select(id => new RecalculateAggregate(_aggregateType, id))); }
        }

        public MapToObjectsSpecProvider<TFact> DeletionMappingSpecificationProvider
        {
            get { return specification => new MapSpecification<IQuery, IEnumerable>(q => q.For(specification).Select(x => x.Id).Select(id => new DestroyAggregate(_aggregateType, id))); }
        }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }
    }
}