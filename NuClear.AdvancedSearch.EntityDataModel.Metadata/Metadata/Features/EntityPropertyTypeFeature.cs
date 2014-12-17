using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityPropertyTypeFeature : IUniqueMetadataFeature
    {
        public EntityPropertyTypeFeature(EntityPropertyType propertyType)
        {
            PropertyType = propertyType;
        }

        public EntityPropertyType PropertyType { get; private set; }
    }
}