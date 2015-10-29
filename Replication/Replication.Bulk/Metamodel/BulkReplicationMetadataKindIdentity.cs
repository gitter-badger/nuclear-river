using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public sealed class BulkReplicationMetadataKindIdentity : MetadataKindIdentityBase<BulkReplicationMetadataKindIdentity>
    {
        public override Uri Id { get; } = Metadata.Id.For(Metadata.Id.DefaultRoot, "BulkReplication");
        public override string Description => "Bulk replication context description";
    }
}