    using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityMappingElement : BaseMetadataElement<EntityMappingElement, EntityMappingElementBuilder>
    {
        private readonly IMetadataElementIdentity _conceptualEntityIdentity;
        private readonly IMetadataElementIdentity _storeEntityIdentity;

        internal EntityMappingElement(
            IMetadataElementIdentity identity, 
            IMetadataElementIdentity conceptualEntityIdentity,
            IMetadataElementIdentity storeEntityIdentity, 
            IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
            _conceptualEntityIdentity = conceptualEntityIdentity;
            _storeEntityIdentity = storeEntityIdentity;
        }

        public IMetadataElementIdentity ConceptualEntityIdentity
        {
            get
            {
                return _conceptualEntityIdentity;
            }
        }

        public IMetadataElementIdentity StoreEntityIdentity
        {
            get
            {
                return _storeEntityIdentity;
            }
        }
    }
}