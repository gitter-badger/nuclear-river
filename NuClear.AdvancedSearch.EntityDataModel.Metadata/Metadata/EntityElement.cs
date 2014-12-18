using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityElement : MetadataElement<EntityElement, EntityElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        internal EntityElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
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

        public IEnumerable<EntityPropertyElement> Keys
        {
            get
            {
                var identityFeature = Features.OfType<EntityIdentityFeature>().FirstOrDefault();
                return identityFeature == null 
                    ? Enumerable.Empty<EntityPropertyElement>() 
                    : identityFeature.IdentifyingProperties;
            }
        }

        public IEnumerable<EntityPropertyElement> Properties
        {
            get
            {
                return Elements.OfType<EntityPropertyElement>();
            }
        }

        public IEnumerable<EntityRelationElement> Relations
        {
            get
            {
                return Elements.OfType<EntityRelationElement>();
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}