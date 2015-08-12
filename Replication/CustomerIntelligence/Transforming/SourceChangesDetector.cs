using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal class SourceChangesDetector : ISourceChangesDetector
    {
        private readonly IMetadataInfo _metadataInfo;
        private readonly IQuery _source;
        private readonly IQuery _target;

        public SourceChangesDetector(IMetadataInfo metadataInfo, IQuery source, IQuery target)
        {
            _metadataInfo = metadataInfo;
            _source = source;
            _target = target;
        }

        public IMergeResult<TTarget> DetectChanges<TTarget>(MapSpecification<IEnumerable, IEnumerable<TTarget>> mapSpec, IReadOnlyCollection<long> ids)
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = mapSpec.Map(_metadataInfo.MapToSourceSpecProvider(ids).Map(_source)).ToArray();
                var targetObjects = mapSpec.Map(_metadataInfo.MapToTargetSpecProvider(ids).Map(_target)).ToArray();
                var result = MergeTool.Merge(sourceObjects, targetObjects);

                return result;
            }
        }
    }
}