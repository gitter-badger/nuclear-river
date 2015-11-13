using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.DataTest.Metamodel
{
    public sealed class SchemaMetadataIdentity : MetadataKindIdentityBase<SchemaMetadataIdentity>
    {
        private readonly Uri _id = Metadata.Id.For("Schema");

        public override Uri Id => _id;

        public override string Description => "Model database schema description";
    }
}
