using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.AdvancedSearch.Common.Metadata.Identities
{
    public sealed class AdvancedSearchIdentity : MetadataKindIdentityBase<AdvancedSearchIdentity>
    {
        private readonly Uri _id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For("AdvancedSearch");

        public override Uri Id
        {
            get
            {
                return _id;
            }
        }

        public override string Description
        {
            get
            {
                return "Advanced Search";
            }
        }
    }
}