using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class BoundedContextElement : BaseMetadataElement<BoundedContextElement, BoundedContextElementBuilder>
    {
        private readonly Lazy<StructuralModelElement> _conceptualModel;
        private readonly Lazy<StructuralModelElement> _storeModel;
        private readonly Lazy<ModelMappingElement> _mapping;

        internal BoundedContextElement(
            IMetadataElementIdentity contextIdentity,
            IMetadataElementIdentity conceptualModelIdentity,
            IMetadataElementIdentity storeModelIdentity,
            IMetadataElementIdentity mappingIdentity,
            IEnumerable<IMetadataFeature> features)
            : base(contextIdentity, features)
        {
            _conceptualModel = new Lazy<StructuralModelElement>(() => conceptualModelIdentity != null 
                ? Elements.OfType<StructuralModelElement>().FirstOrDefault(x => conceptualModelIdentity.Equals(x.Identity))
                : null);
            _storeModel = new Lazy<StructuralModelElement>(() => storeModelIdentity != null 
                ? Elements.OfType<StructuralModelElement>().FirstOrDefault(x => storeModelIdentity.Equals(x.Identity))
                : null);
            _mapping = new Lazy<ModelMappingElement>(() => mappingIdentity != null 
                ? Elements.OfType<ModelMappingElement>().FirstOrDefault(x => mappingIdentity.Equals(x.Identity))
                : null);
        }

        public StructuralModelElement ConceptualModel
        {
            get
            {
                return _conceptualModel.Value;
            }
        }

        public StructuralModelElement StoreModel
        {
            get
            {
                return _storeModel.Value;
            }
        }

        public ModelMappingElement ConceptualToStoreMapping
        {
            get
            {
                return _mapping.Value;
            }
        }
   }
}