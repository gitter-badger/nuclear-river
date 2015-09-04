using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class ValueObjectProcessor<T> : IValueObjectProcessor 
        where T : class, IIdentifiable
    {
        private static readonly MapSpecification<IEnumerable, IEnumerable<IObject>> ValueObjectsChangesDetectionMapSpec =
            new MapSpecification<IEnumerable, IEnumerable<IObject>>(x => x.Cast<IObject>());

        private readonly ValueObjectInfo<T> _metadata;

        public ValueObjectProcessor(ValueObjectInfo<T> metadata)
        {
            _metadata = metadata;
        }

        public void ApplyChanges(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids)
        {
            var changesDetector = new DataChangesDetector<T>(_metadata, query);
            var mergeResult = changesDetector.DetectChanges(ValueObjectsChangesDetectionMapSpec, _metadata.FindSpecificationProvider.Invoke(ids));

            applier.Delete(mergeResult.Complement);
            applier.Create(mergeResult.Difference);
            applier.Update(mergeResult.Intersection);
        }
    }
}