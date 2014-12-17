using System;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityPropertyElementBuilder : MetadataElementBuilder<EntityPropertyElementBuilder, EntityPropertyElement>
    {
        private string _name;

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

        protected override EntityPropertyElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The property name was not specified.");
            }

            return new EntityPropertyElement(new Uri(_name, UriKind.Relative).AsIdentity(), Features);
        }
    }
}