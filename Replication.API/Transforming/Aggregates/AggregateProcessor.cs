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
        private readonly IBulkRepository<T> _repository;
        private readonly AggregateInfo<T> _metadata;
        private readonly DataChangesDetector<T, T> _aggregateChangesDetector;
	    private readonly IReadOnlyCollection<IValueObjectProcessor> _valueObjectProcessors;

	    public AggregateProcessor(AggregateInfo<T> metadata, IValueObjectProcessorFactory valueObjectProcessorFactory, IQuery query, IBulkRepository<T> repository)
        {
            _metadata = metadata;
            _query = query;
            _repository = repository;
            _aggregateChangesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
	        _valueObjectProcessors = _metadata.ValueObjects.Select(valueObjectProcessorFactory.Create).ToArray();
        }

        public void Initialize(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query);

            _repository.Create(aggregatesToCreate);

	        ApplyChangesToValueObjects(ids);
        }

        public void Recalculate(IReadOnlyCollection<long> ids)
        {
			ApplyChangesToValueObjects(ids);

			var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToArray());
            var updateFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Intersection.ToArray());
            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query).ToArray();
            var aggregatesToUpdate = _metadata.MapSpecificationProviderForSource.Invoke(updateFilter).Map(_query).ToArray();
            var aggregatesToDelete = _metadata.MapSpecificationProviderForTarget.Invoke(deleteFilter).Map(_query).ToArray();

            _repository.Delete(aggregatesToDelete);
            _repository.Create(aggregatesToCreate);
            _repository.Update(aggregatesToUpdate);
        }

        public void Destroy(IReadOnlyCollection<long> ids)
        {
			ApplyChangesToValueObjects(ids);

			var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

            var aggregatesToDelete = _metadata.MapSpecificationProviderForTarget.Invoke(deleteFilter).Map(_query);

            _repository.Delete(aggregatesToDelete);
        }

		private void ApplyChangesToValueObjects(IReadOnlyCollection<long> aggregateIds)
		{
			foreach (var processor in _valueObjectProcessors)
			{
				processor.ApplyChanges(aggregateIds);
			}
		}
	}
}