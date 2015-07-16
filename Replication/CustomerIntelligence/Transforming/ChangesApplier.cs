using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal interface IChangesApplier
    {
        IReadOnlyCollection<AggregateOperation> ApplyChanges(MergeTool.MergeResult<long> changes);
    }

    internal class ChangesApplier<TFact> : IChangesApplier where TFact : class, IErmFactObject
    {
        private readonly MapSpecification<IQuery, IQueryable<TFact>> _sourceMapSpecification;
        private readonly IEnumerable<FactDependencyInfo> _dependentAggregates;
        private readonly IQuery _sourceQuery;
        private readonly IQuery _destQuery;
        private readonly IDataMapper _mapper;

        public ChangesApplier(ErmFactInfo factInfo, IQuery sourceQuery, IQuery destQuery, IDataMapper mapper)
        {
            _sourceMapSpecification = ((ErmFactInfo.ErmFactInfoImpl<TFact>)factInfo).MapSpecification;
            _dependentAggregates = factInfo.DependencyInfos;
            _sourceQuery = sourceQuery;
            _destQuery = destQuery;
            _mapper = mapper;
        }

        public IReadOnlyCollection<AggregateOperation> ApplyChanges(MergeTool.MergeResult<long> changes)
        {
            var idsToCreate = changes.Difference.ToArray();
            var idsToUpdate = changes.Intersection.ToArray();
            var idsToDelete = changes.Complement.ToArray();

            var createResult = CreateFact(idsToCreate);
            var updateResult = UpdateFact(idsToUpdate);
            var deleteResult = DeleteFact(idsToDelete);

            return createResult.Concat(updateResult).Concat(deleteResult).ToList();
        }

        private IEnumerable<AggregateOperation> CreateFact(IReadOnlyCollection<long> factIds)
        {
            var sourceQueryable = _sourceMapSpecification.Map(_sourceQuery).Where(Specs.Find.ByIds<TFact>(factIds));
            _mapper.InsertAll<TFact>(sourceQueryable);

            var processor = new DependencyProcessor(_destQuery, _dependentAggregates);
            return processor.ProcessOnCreateFact(factIds);
        }

        private IEnumerable<AggregateOperation> UpdateFact(IReadOnlyCollection<long> factIds)
        {
            var beforeProcessor = new DependencyProcessor(_destQuery, _dependentAggregates.Where(x => !x.IsDirectDependency));
            var before = beforeProcessor.ProcessOnUpdateFact(factIds);

            var sourceQueryable = _sourceMapSpecification.Map(_sourceQuery).Where(Specs.Find.ByIds<TFact>(factIds));
            _mapper.UpdateAll(sourceQueryable);

            var afterProcessor = new DependencyProcessor(_destQuery, _dependentAggregates);
            var after = afterProcessor.ProcessOnUpdateFact(factIds);

            return before.Concat(after);
        }

        private IEnumerable<AggregateOperation> DeleteFact(IReadOnlyCollection<long> factIds)
        {
            var processor = new DependencyProcessor(_destQuery, _dependentAggregates);
            var result = processor.ProcessOnDeleteFact(factIds);

            var sourceQueryable = _destQuery.For(Specs.Find.ByIds<TFact>(factIds));
            _mapper.DeleteAll(sourceQueryable);

            return result;
        }
    }

    internal class DependencyProcessor
    {
        private static readonly Func<FactDependencyInfo, long, AggregateOperation> OperationsFactoryOnCreateFact =
            (dependency, id) =>
            dependency.IsDirectDependency
                ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id);

        private static readonly Func<FactDependencyInfo, long, AggregateOperation> OperationsFactoryOnUpdateFact =
            (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id);

        private static readonly Func<FactDependencyInfo, long, AggregateOperation> OperationsFactoryOnDeleteFact =
            (dependency, id) =>
            dependency.IsDirectDependency
                ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id);

        private readonly IQuery _query;
        private readonly IEnumerable<FactDependencyInfo> _dependencies;

        public DependencyProcessor(IQuery query, IEnumerable<FactDependencyInfo> dependencies)
        {
            _query = query;
            _dependencies = dependencies;
        }

        public IEnumerable<AggregateOperation> ProcessOnCreateFact(IEnumerable<long> factIds)
        {
            return ProcessDependencies(factIds, OperationsFactoryOnCreateFact);
        }

        public IEnumerable<AggregateOperation> ProcessOnUpdateFact(IEnumerable<long> factIds)
        {
            return ProcessDependencies(factIds, OperationsFactoryOnUpdateFact);
        }

        public IEnumerable<AggregateOperation> ProcessOnDeleteFact(IEnumerable<long> factIds)
        {
            return ProcessDependencies(factIds, OperationsFactoryOnDeleteFact);
        }

        private IEnumerable<AggregateOperation> ProcessDependencies(IEnumerable<long> factIds, Func<FactDependencyInfo, long, AggregateOperation> operationFactory)
        {
            var aggregateOperations = new List<AggregateOperation>();
            foreach (var dependency in _dependencies)
            {
                var mapSpec = dependency.DependentAggregateSpecProvider(factIds);
                var dependencyIds = mapSpec.Map(_query).ToArray();
                aggregateOperations.AddRange(dependencyIds.Select(dependencyId => operationFactory(dependency, dependencyId)));
            }

            return aggregateOperations;
        }
    }
}