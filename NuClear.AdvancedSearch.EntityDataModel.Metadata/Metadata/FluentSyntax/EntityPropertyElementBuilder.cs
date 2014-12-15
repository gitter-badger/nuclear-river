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

        public EntityPropertyElementBuilder OfType(TypeCode typeCode)
        {
            AddFeatures(new EntityPropertyTypeFeature(typeCode));
            return this;
        }

        protected override EntityPropertyElement Create()
        {
            return new EntityPropertyElement(new Uri(_name, UriKind.Relative).AsIdentity(), Features);
        }
    }
}