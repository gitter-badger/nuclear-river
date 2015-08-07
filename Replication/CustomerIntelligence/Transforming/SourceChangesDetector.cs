using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal class SourceChangesDetector<TFact> : ISourceChangesDetector<TFact> where TFact : class, IErmFactObject
    {
        private readonly MapSpecification<IQuery, IQueryable<TFact>> _sourceMapSpecification;
        private readonly IQuery _ermQuery;
        private readonly IQuery _factsQuery;

        public SourceChangesDetector(ErmFactInfo factInfo, IQuery ermQuery, IQuery factsQuery)
        {
            _sourceMapSpecification = ((ErmFactInfo.ErmFactInfoImpl<TFact>)factInfo).MapSpecification;
            _ermQuery = ermQuery;
            _factsQuery = factsQuery;
        }

        public IMergeResult<long> DetectChanges(IReadOnlyCollection<long> factIds)
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                var ermData = _sourceMapSpecification.Map(_ermQuery).Select(fact => fact.Id).Where(Specs.Find.ByIds(factIds)).ToArray();
                var factsData = _factsQuery.For(Specs.Find.ByIds<TFact>(factIds)).Select(fact => fact.Id).ToArray();
                var result = MergeTool.Merge(ermData, factsData);

                return result;
            }
        }
    }
}