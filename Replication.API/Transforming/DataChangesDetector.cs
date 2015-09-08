using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    // FIXME {all, 04.09.2015}: Вернуться сюда
    internal class DataChangesDetector<TSource>
    {
        private readonly IQuery _query;
        private readonly MapToObjectsSpecProvider<TSource> _source;
        private readonly MapToObjectsSpecProvider<TSource> _target;

        public DataChangesDetector(IAggregateInfo<TSource> metadataInfo, IQuery query)
        {
            _source = metadataInfo.SourceMappingSpecification;
            _target = metadataInfo.TargetMappingSpecification;
            _query = query;
        }

        public DataChangesDetector(IFactInfo<TSource> metadataInfo, IQuery query)
        {
            _source = metadataInfo.SourceMappingSpecification;
            _target = metadataInfo.TargetMappingSpecification;
            _query = query;
        }

        public DataChangesDetector(IValueObjectInfo<TSource> metadataInfo, IQuery query)
        {
            _source = metadataInfo.SourceMappingSpecification;
            _target = metadataInfo.TargetMappingSpecification;
            _query = query;
        }

        public IMergeResult<TTarget> DetectChanges<TTarget>(MapSpecification<IEnumerable, IEnumerable<TTarget>> mapSpec, FindSpecification<TSource> specification)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = _source.Invoke(specification).Map(_query);
                var targetObjects = _target.Invoke(specification).Map(_query);

                var result = MergeTool.Merge(
                    mapSpec.Map(sourceObjects).ToArray(),
                    mapSpec.Map(targetObjects).ToArray());

                scope.Complete();

                return result;
            }
        }
    }
}