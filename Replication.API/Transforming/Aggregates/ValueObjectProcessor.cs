using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class ValueObjectProcessor<T> : IValueObjectProcessor 
        where T : class, IObject
    {
        // TODO {all, 15.09.2015}: Имеет смысл избавить *Processor от зависимостей IQuery, I*Info, заменить на DataChangesDetector
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _repository;
        private readonly ValueObjectInfo<T> _metadata;
        private readonly DataChangesDetector<T, T> _changesDetector;

        public ValueObjectProcessor(ValueObjectInfo<T> metadata, IQuery query, IBulkRepository<T> repository)
        {
            _metadata = metadata;
            _query = query;
            _repository = repository;
            _changesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
        }

        public void ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _changesDetector.DetectChanges(Specs.Map.ZeroMapping<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            _repository.Delete(mergeResult.Complement);
            _repository.Create(mergeResult.Difference);
            _repository.Update(mergeResult.Intersection);
        }
    }
}