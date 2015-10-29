using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.AdvancedSearch.Replication.Bulk.Metamodel;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.AdvancedSearch.Replication.Bulk.Model
{
    public sealed class BulkReplicationMetadataSource : MetadataSourceBase<BulkReplicationMetadataKindIdentity>
    {
        private static readonly IReadOnlyDictionary<Uri, IMetadataElement> Elements =
            new BulkReplicationMetadataElement[]
            {
                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-fact")
                                              .DefinesReplication
                                              .From("Erm", Schema.Erm)
                                              .To("Facts", Schema.Facts)
                                              .UsingMetadataOfKind<ReplicationMetadataIdentity>("Facts"),

                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-ci")
                                              .DefinesReplication
                                              .From("Facts", Schema.Facts)
                                              .To("CustomerIntelligence", Schema.CustomerIntelligence)
                                              .UsingMetadataOfKind<ReplicationMetadataIdentity>("Aggregates"),

                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-statistics")
                                              .From("Facts", Schema.Facts)
                                              .To("CustomerIntelligence", Schema.CustomerIntelligence)
                                              .UsingMetadataOfKind<StatisticsRecalculationMetadataIdentity>()
                                              .EssentialView("bit.firmcategory")
            }.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata => Elements;
    }
}