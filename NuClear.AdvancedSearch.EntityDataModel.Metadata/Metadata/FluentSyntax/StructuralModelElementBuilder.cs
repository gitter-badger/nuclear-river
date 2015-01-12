using NuClear.Metamodeling.Elements;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class StructuralModelElementBuilder : MetadataElementBuilder<StructuralModelElementBuilder, StructuralModelElement>
    {
        public StructuralModelElementBuilder Elements(params EntityElement[] elements)
        {
            Childs(elements);
            return this;
        }

        protected override StructuralModelElement Create()
        {
            return new StructuralModelElement(null, Features);
        }
    }
}