using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal class SourceChangesDetector<T> : ISourceChangesDetector<T> where T : class, IIdentifiable
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

        public IMergeResult<long> DetectChanges(IReadOnlyCollection<long> ids)
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceIds = _metadataInfo.MapToSourceSpecProvider(ids).Map(_source).Cast<IIdentifiable>().Select(x => x.Id).ToArray();
                var targetIds = _metadataInfo.MapToTargetSpecProvider(ids).Map(_target).Cast<IIdentifiable>().Select(x => x.Id).ToArray();
                var result = MergeTool.Merge(sourceIds, targetIds);

                return result;
            }
        }
    }
}