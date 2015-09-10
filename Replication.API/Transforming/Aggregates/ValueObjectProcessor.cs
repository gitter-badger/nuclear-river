using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class ValueObjectProcessor<T> : IValueObjectProcessor 
        where T : class, IObject
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _applier;
        private readonly ValueObjectInfo<T> _metadata;
        private readonly DataChangesDetector<T, T> _changesDetector;

        public ValueObjectProcessor(ValueObjectInfo<T> metadata, IQuery query, IBulkRepository<T> applier)
        {
            _metadata = metadata;
            _query = query;
            _applier = applier;
            _changesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
        }

        public void ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _changesDetector.DetectChanges(Specs.Map.ZeroMapping<T>(), _metadata.FindSpecificationProvider.Invoke(ids));

            _applier.Delete(mergeResult.Complement);
            _applier.Create(mergeResult.Difference);
            _applier.Update(mergeResult.Intersection);
        }
    }
}