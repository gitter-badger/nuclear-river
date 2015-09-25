using System;
using System.Collections.Generic;
using System.Transactions;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Metadata.Facts;
using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
using NuClear.Storage.Readings;

namespace NuClear.Replication.Core.Facts
{
    public class StatisticsFactImporter<T> : IStatisticsImporter where T : class 
    {
        private readonly ImportStatisticsMetadata<T> _importStatisticsMetadata;
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _repository;

        public StatisticsFactImporter(ImportStatisticsMetadata<T> importStatisticsMetadata, IQuery query, IBulkRepository<T> repository)
        {
            _importStatisticsMetadata = importStatisticsMetadata;
            _query = query;
            _repository = repository;
        }

        public IReadOnlyCollection<RecalculateStatisticsOperation> Import(IStatisticsDto statisticsDto)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _repository.Delete(_query.For(_importStatisticsMetadata.FindSpecificationProvider.Invoke(statisticsDto.ProjectId)));
                _repository.Create(_importStatisticsMetadata.MapSpecification.Map(statisticsDto));

                transaction.Complete();
            }

            return new[] { new RecalculateStatisticsOperation { ProjectId = statisticsDto.ProjectId } };
        }
    }
}