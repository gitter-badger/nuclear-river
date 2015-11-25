using System;
using System.Collections.Generic;

using CustomerIntelligence.Replication.DataTest.Model.Identitites.Connections;

using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For("Erm")
            .HasConnectionString<ErmConnectionStringIdentity>()
            .HasMasterConnectionString<ErmMasterConnectionStringIdentity>()
            .HasSchema(Schema.Erm)
            .HasEntitiesFromNamespace(typeof(Erm::Account).Namespace);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For("Facts")
            .HasConnectionString<FactsConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(Facts::Account).Namespace);

        private static readonly SchemaMetadataElement CustomerIntelligence = SchemaMetadataElement.Config
            .For("CustomerIntelligence")
            .HasConnectionString<CustomerIntelligenceConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence)
            .HasEntitiesFromNamespace(typeof(CI::CategoryGroup).Namespace);

        private static readonly SchemaMetadataElement Bit = SchemaMetadataElement.Config
            .For("Bit")
            .HasConnectionString<BitConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(Bit::FirmCategoryStatistics).Namespace);

        private static readonly SchemaMetadataElement Statistics = SchemaMetadataElement.Config
            .For("Statistics")
            .HasConnectionString<StatisticsConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence)
            .HasEntitiesFromNamespace(typeof(Statistics::FirmCategoryStatistics).Namespace);

        public SchemaMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement>
                        {
                            { Erm.Identity.Id, Erm },
                            { Facts.Identity.Id, Facts },
                            { CustomerIntelligence.Identity.Id, CustomerIntelligence },
                            { Bit.Identity.Id, Bit },
                            { Statistics.Identity.Id, Statistics },
                        };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
