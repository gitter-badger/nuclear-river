using System;
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
        public AggregateDependencyInfo(Type aggregateType,
                                       Func<FindSpecification<TFact>, MapSpecification<IQuery, IEnumerable<long>>> targetMappingSpecificationProvider)
        {
            FindSpecificationProvider = Specs.Find.ByIds<TFact>;

            MapSpecificationProviderOnCreate 
                = MapSpecificationProviderOnUpdate 
                = MapSpecificationProviderOnDelete 
                = specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(q => targetMappingSpecificationProvider.Invoke(specification).Map(q).Select(id => new RecalculateAggregate(aggregateType, id)));
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public bool IsDirectDependency
        {
            get { return false; }
        }

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnCreate { get; private set; }

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnUpdate { get; private set; }

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnDelete { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<TFact>> FindSpecificationProvider { get; private set; }
    }
}