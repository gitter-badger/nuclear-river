using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityPropertyElementBuilder : MetadataElementBuilder<EntityPropertyElementBuilder, EntityPropertyElement>
    {
        private string _name;
        private EntityPropertyType? _propertyType;
        private bool _isNullable;

        private string _enumName;
        private EntityPropertyType? _enumUnderlyingType;
        private readonly Dictionary<string, long> _enumMembers = new Dictionary<string, long>();

        public EntityPropertyElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityPropertyElementBuilder OfType(EntityPropertyType propertyType)
        {
            _propertyType = propertyType;
            return this;
        }

        public EntityPropertyElementBuilder Nullable()
        {
            _isNullable = true;
            return this;
        }

        public EntityPropertyElementBuilder UsingEnum(string name, EntityPropertyType underlyingType = EntityPropertyType.Int32)
        {
            _propertyType = EntityPropertyType.Enum;
            _enumName = name;
            _enumUnderlyingType = underlyingType;
            return this;
        }

        public EntityPropertyElementBuilder WithMember(string name, long value)
        {
            if (_enumUnderlyingType == null)
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
            if (!_propertyType.HasValue)
            {
                throw new InvalidOperationException("The property type was not specified");
            }

            if (_propertyType.Value == EntityPropertyType.Enum && _enumUnderlyingType.HasValue)
            {
                AddFeatures(new EntityPropertyEnumTypeFeature(_enumName, _enumUnderlyingType.Value, _enumMembers));
            }
            else
            {
                AddFeatures(new EntityPropertyTypeFeature(_propertyType.Value));
            }

            if (_isNullable)
            {
                AddFeatures(new EntityPropertyNullableFeature(true));
            }

            return new EntityPropertyElement(_name.AsUri().AsIdentity(), Features);
        }
    }
}