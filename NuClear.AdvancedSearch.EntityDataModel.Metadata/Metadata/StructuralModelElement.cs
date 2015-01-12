using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class StructuralModelElement : MetadataElement<StructuralModelElement, StructuralModelElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        internal StructuralModelElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = identity;
        }

        public IEnumerable<EntityElement> Entities
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