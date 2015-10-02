using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Builders;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public sealed class StructuralModelElement : BaseMetadataElement<StructuralModelElement, StructuralModelElementBuilder>
    {
        private readonly IEnumerable<EntityElement> _rootEntities;

        internal StructuralModelElement(IMetadataElementIdentity identity, IEnumerable<EntityElement> rootEntities, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
            if (rootEntities == null)
            {
                throw new ArgumentNullException("rootEntities");
            }
            _rootEntities = rootEntities;
        }

        public IEnumerable<EntityElement> RootEntities
        {
            get
            {
                return _rootEntities;
            }
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