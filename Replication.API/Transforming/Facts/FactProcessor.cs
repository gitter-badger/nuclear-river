using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class FactProcessor<TFact> : IFactProcessor 
        where TFact : class, IErmFactObject
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TFact> _applier;

        private readonly FactInfo<TFact> _factInfo;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _depencencyProcessors;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _indirectDepencencyProcessors;
        private readonly DataChangesDetector<TFact, TFact> _changesDetector;

        public FactProcessor(FactInfo<TFact> factInfo, IFactDependencyProcessorFactory dependencyProcessorFactory, IQuery query, IBulkRepository<TFact> applier)
        {
            _query = query;
            _applier = applier;
            _factInfo = factInfo;
            _depencencyProcessors = _factInfo.DependencyInfos.Select(dependencyProcessorFactory.Create).ToArray();
            _indirectDepencencyProcessors = _factInfo.DependencyInfos.Where(x => !x.IsDirectDependency).Select(dependencyProcessorFactory.Create).ToArray();
            _changesDetector = new DataChangesDetector<TFact, TFact>(_factInfo.MapSpecificationProviderForSource, _factInfo.MapSpecificationProviderForTarget, _query);
        }

        public IReadOnlyCollection<IOperation> ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var changes = _changesDetector.DetectChanges(Specs.Map.ToIds<TFact>(), _factInfo.FindSpecificationProvider.Invoke(ids));
            var result = new List<IOperation>();

            var idsToCreate = changes.Difference.ToArray();
            var idsToUpdate = changes.Intersection.ToArray();
            var idsToDelete = changes.Complement.ToArray();

            // Create
            CreateFact(idsToCreate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessCreation(idsToCreate)));

            // Update
            result.AddRange(_indirectDepencencyProcessors.SelectMany(x => x.ProcessUpdating(idsToUpdate)));
            UpdateFact(idsToUpdate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessUpdating(idsToUpdate)));

            // Delete
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessDeletion(idsToDelete)));
            DeleteFact(idsToDelete);

            return result;
        }

        private void CreateFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.MapSpecificationProviderForSource.Invoke(spec).Map(_query);
            _applier.Create(sourceQueryable);
        }

        private void UpdateFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.MapSpecificationProviderForSource(spec).Map(_query);
            _applier.Update(sourceQueryable);
        }

        private void DeleteFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var targetQueryable = _factInfo.MapSpecificationProviderForTarget(spec).Map(_query);
            _applier.Delete(targetQueryable);
        }
    }
}