using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityElement : BaseMetadataElement<EntityElement, EntityElementBuilder>
    {
        internal EntityElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
        }
   }
}