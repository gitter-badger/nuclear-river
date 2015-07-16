using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal interface IChangesDetector
    {
        MergeTool.MergeResult<long> DetectChanges(IReadOnlyCollection<long> factIds);
    }

    internal class ChangesDetector<TFact> : IChangesDetector where TFact : class, IErmFactObject
    {
        private readonly MapSpecification<IQuery, IQueryable<TFact>> _sourceMapSpecification;
        private readonly IQuery _ermQuery;
        private readonly IQuery _factsQuery;

        public ChangesDetector(ErmFactInfo factInfo, IQuery ermQuery, IQuery factsQuery)
        {
            _sourceMapSpecification = ((ErmFactInfo.ErmFactInfoImpl<TFact>)factInfo).MapSpecification;
            _ermQuery = ermQuery;
            _factsQuery = factsQuery;
        }

        public MergeTool.MergeResult<long> DetectChanges(IReadOnlyCollection<long> factIds)
        {
            var ermData = _sourceMapSpecification.Map(_ermQuery).Select(fact => fact.Id).Where(Specs.Find.ByIds(factIds)).ToArray();
            var factsData = _factsQuery.For(Specs.Find.ByIds<TFact>(factIds)).Select(fact => fact.Id).ToArray();

            var result = MergeTool.Merge(ermData, factsData);

            return result;
        }
    }
}