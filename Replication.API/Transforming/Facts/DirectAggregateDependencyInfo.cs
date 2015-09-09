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
        public DirectAggregateDependencyInfo(Type aggregateType)
        {
            FindSpecificationProvider = Specs.Find.ByIds<TFact>;

            // FIXME {all, 04.09.2015}: Слабое место - внутри спецификации идентификаторы, затем идём в базу за идентификаторами. Если достать их из спецификации в бузу хдить не потребуется.
            CreationMappingSpecificationProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => q.For(specification).Select(x => x.Id).Select(id => new InitializeAggregate(aggregateType, id)));
            UpdatingMappingSpecificationProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => q.For(specification).Select(x => x.Id).Select(id => new RecalculateAggregate(aggregateType, id)));
            DeletionMappingSpecificationProvider = specification => new MapSpecification<IQuery, IEnumerable>(q => q.For(specification).Select(x => x.Id).Select(id => new DestroyAggregate(aggregateType, id)));
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public bool IsDirectDependency
        {
            get { return true; }
        }

        public MapToObjectsSpecProvider<TFact> CreationMappingSpecificationProvider { get; private set; }

        public MapToObjectsSpecProvider<TFact> UpdatingMappingSpecificationProvider { get; private set; }

        public MapToObjectsSpecProvider<TFact> DeletionMappingSpecificationProvider { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider { get; private set; }
    }
}