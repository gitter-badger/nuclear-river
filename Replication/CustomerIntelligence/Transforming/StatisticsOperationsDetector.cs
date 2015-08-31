using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class StatisticsOperationsDetector
    {
        private readonly IFactInfo _factInfo;
        private readonly IQuery _query;

        public StatisticsOperationsDetector(IFactInfo factInfo, IQuery query)
        {
            _factInfo = factInfo;
            _query = query;
        }

        public IReadOnlyCollection<CalculateStatisticsOperation> DetectOperations(IReadOnlyCollection<long> factIds)
        {
            if (_factInfo.CalculateStatisticsSpecProvider == null)
            {
                return new CalculateStatisticsOperation[0];
            }

            //using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var mapSpec = _factInfo.CalculateStatisticsSpecProvider(factIds);
                var operations = mapSpec.Map(_query).ToArray();

                //scope.Complete();

                return operations;
            }
        }
    }
}