using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public abstract class BaseMetadataElement<TElement, TBuilder> : MetadataElement<TElement, TBuilder>
        where TElement : MetadataElement<TElement, TBuilder> where TBuilder : MetadataElementBuilder<TBuilder, TElement>, new()
    {
        private IMetadataElementIdentity _identity;

        internal protected BaseMetadataElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = identity;
        }

        public override IMetadataElementIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}