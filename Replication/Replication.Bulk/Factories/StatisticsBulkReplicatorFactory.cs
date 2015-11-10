using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.Replicators;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.Factories
{
    public class StatisticsBulkReplicatorFactory<T> : IBulkReplicatorFactory where T : class
    {
        private readonly IQuery _query;
        private readonly DataConnection _dataConnection;

        public StatisticsBulkReplicatorFactory(IQuery query, DataConnection dataConnection)
        {
            _query = query;
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement)
        {
            var valueObjectMetadataElement = (ValueObjectMetadataElement<T>)metadataElement;
            return new[] { new UpdatesBulkReplicator<T>(_query, _dataConnection, valueObjectMetadataElement.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) };
        }

        public void Dispose()
        {
        }
    }
}