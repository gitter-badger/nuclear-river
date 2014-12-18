using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityPropertyElementBuilder : MetadataElementBuilder<EntityPropertyElementBuilder, EntityPropertyElement>
    {
        private string _name;
        private EntityPropertyType? _underlyingType;
        private readonly Dictionary<string, object> _enumMembers = new Dictionary<string, object>();

        public EntityPropertyElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityPropertyElementBuilder NotNull()
        {
            AddFeatures(new EntityPropertyNullableFeature(false));
            return this;
        }

        public EntityPropertyElementBuilder OfType(EntityPropertyType propertyType)
        {
            AddFeatures(new EntityPropertyTypeFeature(propertyType));
            return this;
        }

        public EntityPropertyElementBuilder UsingEnum(EntityPropertyType underlyingType = EntityPropertyType.Int32)
        {
            //AddFeatures(_enumType = new EntityPropertyEnumTypeFeature(propertyType));
            _underlyingType = underlyingType;
            return this;
        }

        public EntityPropertyElementBuilder WithMember<T>(string name, T value) where T : struct
        {
            if (_underlyingType == null)
            {
                throw new InvalidOperationException("The enumeration was not declared.");
            }
            _enumMembers.Add(name, value);
            return this;
        }

        protected override EntityPropertyElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The property name was not specified.");
            }

            return new EntityPropertyElement(
                new Uri(_name, UriKind.Relative).AsIdentity(),
                _underlyingType == null ? Features : Features.Concat(new[] {new EntityPropertyEnumTypeFeature(_underlyingType.Value, _enumMembers) })
                );
        }
    }
}