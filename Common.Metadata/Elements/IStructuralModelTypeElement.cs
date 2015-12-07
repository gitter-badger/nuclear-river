using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public interface IStructuralModelTypeElement : IMetadataElement
    {
        StructuralModelTypeKind TypeKind { get; }
    }
}