using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class SourceChangesApplier<TFact> : ISourceChangesApplier<TFact> where TFact : class, IErmFactObject
    {
        private readonly MapSpecification<IQuery, IQueryable<TFact>> _sourceMapSpecification;
        private readonly IEnumerable<FactDependencyInfo> _dependentAggregates;
        private readonly IQuery _sourceQuery;
        private readonly IQuery _destQuery;
        private readonly IRepository<TFact> _repository;

        public SourceChangesApplier(ErmFactInfo factInfo, IQuery sourceQuery, IQuery destQuery, IRepository<TFact> repository)
        {
            _sourceMapSpecification = ((ErmFactInfo.ErmFactInfoImpl<TFact>)factInfo).MapSpecification;
            _dependentAggregates = factInfo.DependencyInfos;
            _sourceQuery = sourceQuery;
            _destQuery = destQuery;
            _repository = repository;
        }

        public IReadOnlyCollection<AggregateOperation> ApplyChanges(IMergeResult<long> changes)
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
            _repository.AddRange(sourceQueryable);
            _repository.Save();

            var processor = new FactDependencyProcessor(_destQuery, _dependentAggregates);
            return processor.ProcessOnCreateFact(factIds);
        }

        private IEnumerable<AggregateOperation> UpdateFact(IReadOnlyCollection<long> factIds)
        {
            var beforeProcessor = new FactDependencyProcessor(_destQuery, _dependentAggregates.Where(x => !x.IsDirectDependency));
            var before = beforeProcessor.ProcessOnUpdateFact(factIds);

            var sourceQueryable = _sourceMapSpecification.Map(_sourceQuery).Where(Specs.Find.ByIds<TFact>(factIds));
            foreach (var fact in sourceQueryable)
            {
                _repository.Update(fact);
            }
            
            _repository.Save();

            var afterProcessor = new FactDependencyProcessor(_destQuery, _dependentAggregates);
            var after = afterProcessor.ProcessOnUpdateFact(factIds);

            return before.Concat(after);
        }

        private IEnumerable<AggregateOperation> DeleteFact(IReadOnlyCollection<long> factIds)
        {
            var processor = new FactDependencyProcessor(_destQuery, _dependentAggregates);
            var result = processor.ProcessOnDeleteFact(factIds);

            var sourceQueryable = _destQuery.For(Specs.Find.ByIds<TFact>(factIds));
            _repository.DeleteRange(sourceQueryable);
            _repository.Save();

            return result;
        }
    }
}