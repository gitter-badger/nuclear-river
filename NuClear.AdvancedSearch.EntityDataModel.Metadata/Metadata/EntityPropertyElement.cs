using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityPropertyElement : BaseMetadataElement<EntityPropertyElement, EntityPropertyElementBuilder>
    {
        internal EntityPropertyElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
        }

        public EntityPropertyType PropertyType
        {
            get
            {
                return ResolveFeature<EntityPropertyTypeFeature, EntityPropertyType>(
                    f => f.PropertyType,
                    () => { throw new InvalidOperationException("The type was not specified."); });
            }
        }

        public bool IsNullable
        {
            get
            {
                return ResolveFeature<EntityPropertyNullableFeature, bool>(f => f.IsNullable);
            }
        }

        public string EnumName
        {
            get
            {
                ThrowIfNotEnum();
                return ResolveFeature<EntityPropertyEnumTypeFeature, string>(f => f.Name);
            }
        }

        public EntityPropertyType EnumUnderlyingType
        {
            get
            {
                ThrowIfNotEnum();
                return ResolveFeature<EntityPropertyEnumTypeFeature, EntityPropertyType>(f => f.UnderlyingType);
            }
        }

        public IReadOnlyDictionary<string, long> EnumMembers
        {
            get
            {
                ThrowIfNotEnum();
                return ResolveFeature<EntityPropertyEnumTypeFeature, IReadOnlyDictionary<string, long>>(f => f.Members);
            }
        }

        private void ThrowIfNotEnum()
        {
            if (PropertyType != EntityPropertyType.Enum)
            {
                throw new InvalidOperationException("The specified element is not of an enum type.");
            }
        }
    }
}