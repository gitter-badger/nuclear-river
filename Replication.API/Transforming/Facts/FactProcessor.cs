using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class FactProcessor<TFact> : IFactProcessor where TFact : class, IErmFactObject
    {
        private readonly FactInfo<TFact> _factInfo;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _depencencyProcessors;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _indirectDepencencyProcessors;

        public FactProcessor(FactInfo<TFact> factInfo, IFactDependencyProcessorFactory dependencyProcessorFactory)
        {
            _factInfo = factInfo;
            _depencencyProcessors = _factInfo.DependencyInfos.Select(dependencyProcessorFactory.Create).ToList();
            _indirectDepencencyProcessors = _factInfo.DependencyInfos.Where(x => !x.IsDirectDependency).Select(dependencyProcessorFactory.Create).ToList();
        }

        public IReadOnlyCollection<IOperation> ApplyChanges(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var changesDetector = new DataChangesDetector<TFact>(_factInfo, query);
            var changes = changesDetector.DetectChanges(Specs.Map.ToIds, _factInfo.FindSpecificationProvider.Invoke(ids));
            var result = new List<IOperation>();

            var idsToCreate = changes.Difference.ToList();
            var idsToUpdate = changes.Intersection.ToList();
            var idsToDelete = changes.Complement.ToList();

            // Create
            CreateFact(query, applier, idsToCreate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessCreation(query, idsToCreate)));

            // Update
            result.AddRange(_indirectDepencencyProcessors.SelectMany(x => x.ProcessUpdating(query, idsToUpdate)));
            UpdateFact(query, applier, idsToUpdate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessUpdating(query, idsToUpdate)));

            // Delete
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessDeletion(query, idsToDelete)));
            DeleteFact(query, applier, idsToDelete);

            return result;
        }

        private void CreateFact(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.SourceMappingProvider.Invoke(spec).Map(query).Cast<TFact>();
            applier.Create(sourceQueryable);
        }

        private void UpdateFact(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.SourceMappingProvider(spec).Map(query).Cast<TFact>();
            applier.Update(sourceQueryable);
        }

        private void DeleteFact(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var targetQueryable = _factInfo.TargetMappingProvider(spec).Map(query).Cast<TFact>();
            applier.Delete(targetQueryable);
        }
    }
}