using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityPropertyElement : MetadataElement<EntityPropertyElement, EntityPropertyElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        internal EntityPropertyElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
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

        public EntityPropertyType PropertyType
        {
            get
            {
                var typeFeature = Features.OfType<EntityPropertyTypeFeature>().FirstOrDefault();
                if (typeFeature == null)
                {
                    throw new InvalidOperationException("The type was not specified.");
                }
                return typeFeature.PropertyType;
            }
        }

        public bool IsNullable
        {
            get
            {
                var nullableFeature = Features.OfType<EntityPropertyNullableFeature>().FirstOrDefault();
                var isNullable = nullableFeature == null || nullableFeature.IsNullable;
                return isNullable;
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}