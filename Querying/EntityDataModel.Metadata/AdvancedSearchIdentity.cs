using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
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