using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Provider;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    internal class PrefillStatisticsContext : ITestAction
    {
        private readonly DataConnectionFactory _connectionFactory;
        private readonly IDictionary<string, SchemaMetadataElement> _schemaMetadata;

        public PrefillStatisticsContext(IMetadataProvider metadataProvider, DataConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _schemaMetadata = metadataProvider.GetMetadataSet<SchemaMetadataIdentity>().Metadata.Values.Cast<SchemaMetadataElement>().ToDictionary(x => x.Context, x => x);
        }

        public void Act()
        {
            using (var source = _connectionFactory.CreateConnection(_schemaMetadata["CustomerIntelligence"]))
            using (var target = _connectionFactory.CreateConnection(_schemaMetadata["Statistics"]))
            {
                var transformed = source.GetTable<CI::FirmCategory>()
                    .Select(x => new Statistics::FirmCategoryStatistics { FirmId = x.FirmId, CategoryId = x.CategoryId })
                    .ToArray();
                target.GetTable<Statistics::FirmCategoryStatistics>().BulkCopy(transformed);
            }
        }
    }
}