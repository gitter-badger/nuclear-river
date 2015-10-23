using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public sealed class MassReplicationMetadataKindIdentity : MetadataKindIdentityBase<MassReplicationMetadataKindIdentity>
    {
        public override Uri Id { get; } = Metadata.Id.For(Metadata.Id.DefaultRoot, "MassReplication");
        public override string Description => "Mass replication context description";
    }
}