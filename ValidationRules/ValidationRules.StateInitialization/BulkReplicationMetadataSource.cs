using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.ValidationRules.Domain;
using NuClear.ValidationRules.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.Metadata;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization
{
    public sealed class BulkReplicationMetadataSource : MetadataSourceBase<BulkReplicationMetadataKindIdentity>
    {
        private static readonly IReadOnlyDictionary<Uri, IMetadataElement> Elements =
            new BulkReplicationMetadataElement[]
            {
                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-facts")
                                              .From(ErmConnectionStringIdentity.Instance, Schema.Erm)
                                              .To(FactsConnectionStringIdentity.Instance, Schema.Facts)
                                              .UsingMetadataOfKind<ReplicationMetadataIdentity>("PriceContext.Facts"),

                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-config")
                                              .From(@"C:\dev\erm\CompositionRoots\Source\2Gis.Erm.API.WCF.OrderValidation\orderValidation.config")
                                              .To(FactsConnectionStringIdentity.Instance, Schema.Facts)
                                              .UsingMetadataOfKind<ReplicationMetadataIdentity>("PriceContext.Config"),

            }.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata => Elements;
    }
}