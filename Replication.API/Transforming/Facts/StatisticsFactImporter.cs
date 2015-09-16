using System;
using System.Collections.Generic;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class StatisticsFactImporter<TStatisticsFact>
        where TStatisticsFact : class
    {
        private readonly IStatisticsFactInfo<TStatisticsFact> _statisticsFactInfo;
        private readonly IQuery _query;
        private readonly IBulkRepository<TStatisticsFact> _repository;

        public StatisticsFactImporter(IStatisticsFactInfo<TStatisticsFact> statisticsFactInfo, IQuery query, IBulkRepository<TStatisticsFact> repository)
        {
            _statisticsFactInfo = statisticsFactInfo;
            _query = query;
            _repository = repository;
        }

        public IReadOnlyCollection<CalculateStatisticsOperation> Import(IStatisticsDto statisticsDto)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _repository.Delete(_query.For(_statisticsFactInfo.FindSpecificationProvider.Invoke(statisticsDto.ProjectId)));
                _repository.Create(_statisticsFactInfo.MapSpecification.Map(statisticsDto));

                transaction.Complete();
            }

            return new[] { new CalculateStatisticsOperation { ProjectId = statisticsDto.ProjectId } };
        }
    }
}