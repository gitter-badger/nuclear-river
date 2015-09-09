using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class ValueObjectProcessor<T> : IValueObjectProcessor 
        where T : class, IObject
    {
        private static readonly MapSpecification<IEnumerable, IEnumerable<IObject>> ValueObjectsChangesDetectionMapSpec =
            new MapSpecification<IEnumerable, IEnumerable<IObject>>(x => x.Cast<IObject>());

        private readonly IQuery _query;
        private readonly IDataChangesApplier<T> _applier;
        private readonly ValueObjectInfo<T> _metadata;

        public ValueObjectProcessor(ValueObjectInfo<T> metadata, IQuery query, IDataChangesApplier<T> applier)
        {
            _metadata = metadata;
            _query = query;
            _applier = applier;
        }

        public void ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var changesDetector = new DataChangesDetector<T>(_metadata, _query);
            var mergeResult = changesDetector.DetectChanges(ValueObjectsChangesDetectionMapSpec, _metadata.FindSpecificationProvider.Invoke(ids));

            _applier.Delete(mergeResult.Complement);
            _applier.Create(mergeResult.Difference);
            _applier.Update(mergeResult.Intersection);
        }
    }
}