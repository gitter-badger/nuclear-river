using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class BoundedContextElement : MetadataElement<BoundedContextElement, BoundedContextElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        internal BoundedContextElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = identity;
        }

        public IEnumerable<EntityElement> RootEntities
        {
            get
            {
                return Elements.OfType<EntityElement>();
            }
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