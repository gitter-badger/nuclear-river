using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityPropertyTypeFeature : IUniqueMetadataFeature
    {
        public EntityPropertyTypeFeature(TypeCode typeCode)
        {
            TypeCode = typeCode;
        }

        public TypeCode TypeCode { get; private set; }
    }
}