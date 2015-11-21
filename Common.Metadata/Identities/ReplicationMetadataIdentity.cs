using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.AdvancedSearch.Common.Metadata.Identities
{
    public class ReplicationMetadataIdentity : MetadataKindIdentityBase<ReplicationMetadataIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "Replication");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Replication system descriptive metadata"; }
        }
    }
}