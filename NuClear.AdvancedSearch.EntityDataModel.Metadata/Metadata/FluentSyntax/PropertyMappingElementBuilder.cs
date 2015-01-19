using NuClear.Metamodeling.Elements;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class PropertyMappingElementBuilder : MetadataElementBuilder<PropertyMappingElementBuilder, PropertyMappingElement>
    {
        public PropertyMappingElementBuilder Map(string conceptualPropertyName, string storePropertyName)
        {
            return this;
        }

        protected override PropertyMappingElement Create()
        {
            return new PropertyMappingElement(null, Features);
        }
    }
}