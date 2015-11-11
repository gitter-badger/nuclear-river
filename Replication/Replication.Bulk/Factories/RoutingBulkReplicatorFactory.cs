using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.References;
using NuClear.Replication.Bulk.Metadata;
using NuClear.Replication.Bulk.Replicators;
using NuClear.Replication.Bulk.Storage;

namespace NuClear.Replication.Bulk.Factories
{
    public class RoutingBulkReplicatorFactory : IBulkReplicatorFactory
    {
        private readonly DataConnection _sourceDataConnection;
        private readonly DataConnection _targetDataConnection;

        private static readonly IReadOnlyDictionary<Type, Type> RoutingDictionary =
            new Dictionary<Type, Type>
            {
                { typeof(FactMetadata<>), typeof(FactBulkReplicatorFactory<>) },
                { typeof(AggregateMetadata<>), typeof(AggregatesBulkReplicatorFactory<>) },
                { typeof(ValueObjectMetadataElement<>), typeof(ValueObjectMetadataElement<>) },
                { typeof(StatisticsRecalculationMetadata<>), typeof(ValueObjectMetadataElement<>) }
            };

        private RoutingBulkReplicatorFactory(DataConnection sourceDataConnection, DataConnection targetDataConnection)
        {
            _sourceDataConnection = sourceDataConnection;
            _targetDataConnection = targetDataConnection;
        }

        public static IBulkReplicatorFactory Create(BulkReplicationMetadataElement bulkReplicationMetadata)
        {
            var sourceStorageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.From);
            var targetStorageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.To);
            return new RoutingBulkReplicatorFactory(CreateConnection(sourceStorageDescriptor), CreateConnection(targetStorageDescriptor));
        }

        IReadOnlyCollection<IBulkReplicator> IBulkReplicatorFactory.Create(IMetadataElement metadataElement)
        {
            var metadataElementType = metadataElement.GetType();

            Type factoryType;
            if (!RoutingDictionary.TryGetValue(metadataElementType.GetGenericTypeDefinition(), out factoryType))
            {
                throw new NotSupportedException($"Bulk replication is not supported for the mode described with {metadataElement}");
            }

            var objType = metadataElementType.GenericTypeArguments[0];
            var factory = (IBulkReplicatorFactory)Activator.CreateInstance(factoryType.MakeGenericType(objType), new LinqToDbQuery(_sourceDataConnection), _targetDataConnection);
            return factory.Create(metadataElement);
        }

        public void Dispose()
        {
            _sourceDataConnection.Dispose();
            _targetDataConnection.Dispose();
        }

        private static DataConnection CreateConnection(StorageDescriptorFeature storageDescriptorFeature)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[storageDescriptorFeature.ConnectionStringName];
            var connection = new DataConnection(connectionString.ProviderName, connectionString.ConnectionString);
            connection.AddMappingSchema(storageDescriptorFeature.MappingSchema);
            connection.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
            return connection;
        }
    }
}