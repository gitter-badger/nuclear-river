using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
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
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(q => dependentAggregateSpecProvider
                                                                                                .Invoke(specification)
                                                                                                .Map(q)
                                                                                                .Select(id => new RecalculateAggregate(typeof(TAggregate), id)));
            AddFeatures(new IndirectlyDependentAggregateFeature<T>(mapSpecificationProvider));
            return this;
        }

        public FactMetadataBuilder<T> HasMatchedAggregate<TAggregate>()
            where TAggregate : class, IIdentifiable 
        {
            // FIXME {all, 03.09.2015}: TAggregate заменить на идентификатор типа
            // FIXME {all, 04.09.2015}: Слабое место - внутри спецификации идентификаторы, затем идём в базу за идентификаторами. Если достать их из спецификации в бузу хдить не потребуется.
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnCreate =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => q.For(specification)
                                           .Select(x => x.Id)
                                           .Select(id => new InitializeAggregate(typeof(TAggregate), id)));

            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnUpdate =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => q.For(specification)
                                           .Select(x => x.Id)
                                           .Select(id => new RecalculateAggregate(typeof(TAggregate), id)));
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnDelete =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => q.For(specification)
                                           .Select(x => x.Id)
                                           .Select(id => new DestroyAggregate(typeof(TAggregate), id)));

            AddFeatures(new DirectlyDependentAggregateFeature<T>(mapSpecificationProviderOnCreate, mapSpecificationProviderOnUpdate, mapSpecificationProviderOnDelete));
            return this;
        }

        public FactMetadataBuilder<T> LeadsToStatisticsCalculation(Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> provider)
        {
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => provider.Invoke(specification)
                                                  .Map(q)
                                                  .Select(tuple => new RecalculateStatisticsOperation
                                                                   {
                                                                       ProjectId = tuple.Item1,
                                                                       CategoryId = tuple.Item2
                                                                   }));

            AddFeatures(new DependentStatisticsFeature<T>(mapSpecificationProvider));
            return this;
        }
    }
}