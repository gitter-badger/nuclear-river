using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class FactChangesApplier<TFact> : IFactChangesApplier<TFact> where TFact : class, IErmFactObject
    {
        private readonly IFactInfo _factInfo;
        private readonly IQuery _query;
        private readonly IRepository<TFact> _repository;

        public FactChangesApplier(IFactInfo factInfo, IQuery query, IRepository<TFact> repository)
        {
            _factInfo = factInfo;
            _query = query;
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
            var sourceQueryable = _factInfo.MapToSourceSpecProvider(factIds).Map(_query).Cast<TFact>();
            _repository.AddRange(sourceQueryable.AsUntransactional());
            _repository.Save();

            var processor = new FactDependencyProcessor(_query, _factInfo.DependencyInfos);
            return processor.ProcessOnCreateFact(factIds);
        }

        private IEnumerable<AggregateOperation> UpdateFact(IReadOnlyCollection<long> factIds)
        {
            var beforeProcessor = new FactDependencyProcessor(_query, _factInfo.DependencyInfos.Where(x => !x.IsDirectDependency));
            var before = beforeProcessor.ProcessOnUpdateFact(factIds);

            var sourceQueryable = _factInfo.MapToSourceSpecProvider(factIds).Map(_query).Cast<TFact>();
            foreach (var fact in sourceQueryable.AsUntransactional())
            {
                _repository.Update(fact);
            }
            
            _repository.Save();

            var afterProcessor = new FactDependencyProcessor(_query, _factInfo.DependencyInfos);
            var after = afterProcessor.ProcessOnUpdateFact(factIds);

            return before.Concat(after);
        }

        private IEnumerable<AggregateOperation> DeleteFact(IReadOnlyCollection<long> factIds)
        {
            var processor = new FactDependencyProcessor(_query, _factInfo.DependencyInfos);
            var result = processor.ProcessOnDeleteFact(factIds);

            var targetQueryable = _factInfo.MapToTargetSpecProvider(factIds).Map(_query).Cast<TFact>();
            _repository.DeleteRange(targetQueryable.AsUntransactional());
            _repository.Save();

            return result;
        }
    }
}