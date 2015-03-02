using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public interface IStructuralModelTypeElement : IMetadataElement
    {
        StructuralModelTypeKind TypeKind { get; }
    }
}