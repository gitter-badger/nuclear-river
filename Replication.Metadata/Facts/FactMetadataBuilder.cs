using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Metadata.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata.Facts
{
    public class FactMetadataBuilder<T> : MetadataElementBuilder<FactMetadataBuilder<T>, FactMetadata<T>>
        where T : class, IIdentifiable
    {
        private MapSpecification<IQuery, IQueryable<T>> _sourceMappingSpecification;

        protected override FactMetadata<T> Create()
        {
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource =
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => _sourceMappingSpecification.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<T>>(q => q.For<T>());
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget = 
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification));

            return new FactMetadata<T>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, Specs.Find.ByIds<T>, Features);
        }

        public FactMetadataBuilder<T> HasSource(MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            return this;
        }

        public FactMetadataBuilder<T> HasDependentAggregate<TAggregate>(
            Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
            
            where TAggregate : class, IIdentifiable 
        {
            // FIXME {all, 03.09.2015}: TAggregate заменить на идентификатор типа
            AddFeatures(new IndirectlyDependentAggregateFeature<T>(typeof(TAggregate), dependentAggregateSpecProvider));
            return this;
        }

        public FactMetadataBuilder<T> HasMatchedAggregate<TAggregate>()
            where TAggregate : class, IIdentifiable 
        {
            // FIXME {all, 03.09.2015}: TAggregate заменить на идентификатор типа
            AddFeatures(new DirectlyDependentAggregateFeature<T>(typeof(TAggregate)));
            return this;
        }

        public FactMetadataBuilder<T> LeadsToStatisticsCalculation(Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> provider)
        {
            AddFeatures(new DependentStatisticsFeature<T>(provider));
            return this;
        }
    }
}