using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class StructuralModelElement : BaseMetadataElement<StructuralModelElement, StructuralModelElementBuilder>
    {
        internal StructuralModelElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
        }

        public IEnumerable<EntityElement> Entities
        {
            get
            {
                return Elements.OfType<EntityElement>();
            }
        }
   }
}