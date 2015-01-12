using System;
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
        private readonly Lazy<StructuralModelElement> _conceptualModel;
        private readonly Lazy<StructuralModelElement> _storeModel;

        internal BoundedContextElement(
            IMetadataElementIdentity contextIdentity,
            IMetadataElementIdentity conceptualModelIdentity,
            IMetadataElementIdentity storeModelIdentity,
            IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = contextIdentity;
            _conceptualModel = new Lazy<StructuralModelElement>(() => conceptualModelIdentity != null 
                ? Elements.OfType<StructuralModelElement>().FirstOrDefault(x => conceptualModelIdentity.Equals(x.Identity))
                : null);
            _storeModel = new Lazy<StructuralModelElement>(() => storeModelIdentity != null 
                ? Elements.OfType<StructuralModelElement>().FirstOrDefault(x => storeModelIdentity.Equals(x.Identity))
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

        //public object ConceptualToStoreMapping;

        public IEnumerable<EntityElement> Entities
        {
            get
            {
                return ConceptualModel.Elements.OfType<EntityElement>();
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