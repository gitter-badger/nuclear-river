using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityRelationElement : MetadataElement<EntityRelationElement, EntityRelationElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        internal EntityRelationElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = identity;
        }

        public EntityRelationCardinality Cardinality
        {
            get
            {
                var cardinalityFeature = Features.OfType<EntityRelationCardinalityFeature>().SingleOrDefault();
                if (cardinalityFeature == null)
                {
                    throw new InvalidOperationException("The cardinality was not specified.");
                }
                return cardinalityFeature.Cardinality;
            }
        }

        public EntityElement Target
        {
            get
            {
                var target = Elements.OfType<EntityElement>().SingleOrDefault();
                if (target == null)
                {
                    throw new InvalidOperationException("The target entity was not specified.");
                }
                return target;
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