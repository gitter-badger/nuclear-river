using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class PropertyMappingElement : BaseMetadataElement<PropertyMappingElement, PropertyMappingElementBuilder>
    {
        internal PropertyMappingElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
        }
    }
}