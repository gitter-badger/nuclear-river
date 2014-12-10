using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.EntityDataModel
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