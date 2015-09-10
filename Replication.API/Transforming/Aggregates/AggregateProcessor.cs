using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class AggregateProcessor<T> : IAggregateProcessor 
        where T : class, IIdentifiable
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _applier;
        private readonly AggregateInfo<T> _metadata;
        private readonly DataChangesDetector<T, T> _aggregateChangesDetector;

        public AggregateProcessor(AggregateInfo<T> metadata, IQuery query, IBulkRepository<T> applier)
        {
            _metadata = metadata;
            _query = query;
            _applier = applier;
            _aggregateChangesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
        }

        public void Initialize(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query);

            _applier.Create(aggregatesToCreate);
        }

        public void Recalculate(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToArray());
            var updateFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Intersection.ToArray());
            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query);
            var aggregatesToUpdate = _metadata.MapSpecificationProviderForSource.Invoke(updateFilter).Map(_query);
            var aggregatesToDelete = _metadata.MapSpecificationProviderForTarget.Invoke(deleteFilter).Map(_query);

            _applier.Delete(aggregatesToDelete);
            _applier.Create(aggregatesToCreate);
            _applier.Update(aggregatesToUpdate);
        }

        public void Destroy(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

            var aggregatesToDelete = _metadata.MapSpecificationProviderForTarget.Invoke(deleteFilter).Map(_query);

            _applier.Delete(aggregatesToDelete);
        }
    }
}