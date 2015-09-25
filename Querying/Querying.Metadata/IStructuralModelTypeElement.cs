using NuClear.Metamodeling.Elements;

namespace NuClear.Querying.Metadata
{
    public interface IStructuralModelTypeElement : IMetadataElement
    {
        StructuralModelTypeKind TypeKind { get; }
    }
}