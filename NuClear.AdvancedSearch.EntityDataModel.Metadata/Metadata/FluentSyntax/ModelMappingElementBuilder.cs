using NuClear.Metamodeling.Elements;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class ModelMappingElementBuilder : MetadataElementBuilder<ModelMappingElementBuilder, ModelMappingElement>
    {
        public ModelMappingElementBuilder Mappings(params EntityMappingElement[] entityMappings)
        {
            Childs(entityMappings);
            return this;
        }

        protected override ModelMappingElement Create()
        {
            return new ModelMappingElement(null, Features);
        }
    }
}