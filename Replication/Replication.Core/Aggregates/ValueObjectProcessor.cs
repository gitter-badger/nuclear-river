using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ValueObjectProcessor<T> : IValueObjectProcessor 
        where T : class, IObject
    {
        private readonly IBulkRepository<T> _repository;
        private readonly ValueObjectMetadataElement<T> _metadata;
        private readonly DataChangesDetector<T, T> _changesDetector;

        // TODO {all, 15.09.2015}: Имеет смысл избавить *Processor от зависимостей IQuery, I*Info, заменить на DataChangesDetector
        public ValueObjectProcessor(ValueObjectMetadataElement<T> metadata, IQuery query, IBulkRepository<T> repository)
        {
            _metadata = metadata;
            _repository = repository;
            _changesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, query);
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