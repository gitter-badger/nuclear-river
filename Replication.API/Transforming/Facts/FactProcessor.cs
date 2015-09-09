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
        private readonly IDataChangesApplier<TFact> _applier;

        private readonly FactInfo<TFact> _factInfo;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _depencencyProcessors;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _indirectDepencencyProcessors;
        private readonly DataChangesDetector<TFact> _changesDetector;

        public FactProcessor(FactInfo<TFact> factInfo, IFactDependencyProcessorFactory dependencyProcessorFactory, IQuery query, IDataChangesApplier<TFact> applier)
        {
            _query = query;
            _applier = applier;
            _factInfo = factInfo;
            _depencencyProcessors = _factInfo.DependencyInfos.Select(dependencyProcessorFactory.Create).ToList();
            _indirectDepencencyProcessors = _factInfo.DependencyInfos.Where(x => !x.IsDirectDependency).Select(dependencyProcessorFactory.Create).ToList();
            _changesDetector = new DataChangesDetector<TFact>(_factInfo, _query);
        }

        public IReadOnlyCollection<IOperation> ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var changes = _changesDetector.DetectChanges(Specs.Map.ToIds, _factInfo.FindSpecificationProvider.Invoke(ids));
            var result = new List<IOperation>();

            var idsToCreate = changes.Difference.ToList();
            var idsToUpdate = changes.Intersection.ToList();
            var idsToDelete = changes.Complement.ToList();

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
            var sourceQueryable = _factInfo.SourceMappingProvider.Invoke(spec).Map(_query).Cast<TFact>();
            _applier.Create(sourceQueryable);
        }

        private void UpdateFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.SourceMappingProvider(spec).Map(_query).Cast<TFact>();
            _applier.Update(sourceQueryable);
        }

        private void DeleteFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var targetQueryable = _factInfo.TargetMappingProvider(spec).Map(_query).Cast<TFact>();
            _applier.Delete(targetQueryable);
        }
    }
}