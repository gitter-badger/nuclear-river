using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Replication.Bulk.Replicators;
using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Factories
{
    public class ValueObjectsBulkReplicatorFactory<T> : IBulkReplicatorFactory where T : class
    {
        private readonly IQuery _query;
        private readonly DataConnection _dataConnection;

        public ValueObjectsBulkReplicatorFactory(IQuery query, DataConnection dataConnection)
        {
            _query = query;
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement)
        {
            var statisticsRecalculationMetadata = (StatisticsRecalculationMetadata<T>)metadataElement;
            return new[] { new InsertsBulkReplicator<T>(_query, _dataConnection, statisticsRecalculationMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) };
        }

        public void Dispose()
        {
        }
    }
}