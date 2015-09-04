using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class AggregateProcessor<T> : IAggregateProcessor where T : class, IIdentifiable
    {
        private readonly AggregateInfo<T> _metadata;

        public AggregateProcessor(AggregateInfo<T> metadata)
        {
            _metadata = metadata;
        }

        public void Initialize(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var aggregateChangesDetector = new DataChangesDetector<T>(_metadata, query);
            var mergeResult = aggregateChangesDetector.DetectChanges(Specs.Map.ToIds, _metadata.FindSpecificationProvider.Invoke(ids));

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToList());

            var aggregatesToCreate = _metadata.SourceMappingSpecification.Invoke(createFilter).Map(query);

            applier.Create(aggregatesToCreate);
        }

        public void Recalculate(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var aggregateChangesDetector = new DataChangesDetector<T>(_metadata, query);
            var mergeResult = aggregateChangesDetector.DetectChanges(Specs.Map.ToIds, _metadata.FindSpecificationProvider.Invoke(ids));

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToList());
            var updateFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Intersection.ToList());
            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToList());

            var aggregatesToCreate = _metadata.SourceMappingSpecification.Invoke(createFilter).Map(query);
            var aggregatesToUpdate = _metadata.SourceMappingSpecification.Invoke(updateFilter).Map(query);
            var aggregatesToDelete = _metadata.TargetMappingSpecification.Invoke(deleteFilter).Map(query);

            applier.Delete(aggregatesToDelete);
            applier.Create(aggregatesToCreate);
            applier.Update(aggregatesToUpdate);
        }

        public void Destroy(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var aggregateChangesDetector = new DataChangesDetector<T>(_metadata, query);
            var mergeResult = aggregateChangesDetector.DetectChanges(Specs.Map.ToIds, _metadata.FindSpecificationProvider.Invoke(ids));

            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToList());

            var aggregatesToDelete = _metadata.TargetMappingSpecification.Invoke(deleteFilter).Map(query);

            applier.Delete(aggregatesToDelete);
        }
    }
}