using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class ModelMappingElement : BaseMetadataElement<ModelMappingElement, ModelMappingElementBuilder>
    {
        internal ModelMappingElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
        }

        public IEnumerable<EntityMappingElement> Mappings()
        {
            return Elements.OfType<EntityMappingElement>();
        }
    }
}