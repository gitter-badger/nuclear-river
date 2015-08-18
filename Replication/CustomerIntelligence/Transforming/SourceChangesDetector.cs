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
        private readonly IQuery _query;

        public SourceChangesDetector(IMetadataInfo metadataInfo, IQuery query)
        {
            _metadataInfo = metadataInfo;
            _query = query;
        }

        public IMergeResult<TTarget> DetectChanges<TTarget>(MapSpecification<IEnumerable, IEnumerable<TTarget>> mapSpec, IReadOnlyCollection<long> ids)
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = mapSpec.Map(_metadataInfo.MapToSourceSpecProvider(ids).Map(_query)).ToArray();
                var targetObjects = mapSpec.Map(_metadataInfo.MapToTargetSpecProvider(ids).Map(_query)).ToArray();
                var result = MergeTool.Merge(sourceObjects, targetObjects);

                return result;
            }
        }
    }
}