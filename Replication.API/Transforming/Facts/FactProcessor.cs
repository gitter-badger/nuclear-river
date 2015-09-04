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
        private readonly IFactDependencyProcessorFactory _dependencyProcessorFactory;

        public FactProcessor(FactInfo<TFact> factInfo, IFactDependencyProcessorFactory dependencyProcessorFactory)
        {
            _factInfo = factInfo;
            _dependencyProcessorFactory = dependencyProcessorFactory;
        }

        public IReadOnlyCollection<IOperation> ApplyChanges(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var changesDetector = new DataChangesDetector<TFact>(_factInfo, query);
            var changes = changesDetector.DetectChanges(Specs.Map.ToIds, _factInfo.FindSpecificationProvider.Invoke(ids));
            var result = new List<IOperation>();

            var idsToCreate = changes.Difference.ToList();
            var idsToUpdate = changes.Intersection.ToList();
            var idsToDelete = changes.Complement.ToList();

            var depencencyProcessors = _factInfo.DependencyInfos.Select(x => _dependencyProcessorFactory.Create(x)).ToList();
            var indirectDepencencyProcessors = _factInfo.DependencyInfos.Where(x => !x.IsDirectDependency).Select(x => _dependencyProcessorFactory.Create(x)).ToList();

            // Create
            CreateFact(query, applier, idsToCreate);
            result.AddRange(depencencyProcessors.SelectMany(x => x.ProcessCreation(query, idsToCreate)));

            // Update
            result.AddRange(indirectDepencencyProcessors.SelectMany(x => x.ProcessUpdating(query, idsToUpdate)));
            UpdateFact(query, applier, idsToUpdate);
            result.AddRange(depencencyProcessors.SelectMany(x => x.ProcessUpdating(query, idsToUpdate)));

            // Delete
            result.AddRange(depencencyProcessors.SelectMany(x => x.ProcessDeletion(query, idsToDelete)));
            DeleteFact(query, applier, idsToDelete);

            return result;
        }

        private void CreateFact(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.SourceMappingSpecification.Invoke(spec).Map(query).Cast<TFact>();
            applier.Create(sourceQueryable);
        }

        private void UpdateFact(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factInfo.SourceMappingSpecification(spec).Map(query).Cast<TFact>();
            applier.Update(sourceQueryable);
        }

        private void DeleteFact(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var spec = _factInfo.FindSpecificationProvider.Invoke(ids);
            var targetQueryable = _factInfo.TargetMappingSpecification(spec).Map(query).Cast<TFact>();
            applier.Delete(targetQueryable);
        }
    }
}