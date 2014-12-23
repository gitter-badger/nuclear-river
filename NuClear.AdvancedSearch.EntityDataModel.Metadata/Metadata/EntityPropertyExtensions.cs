using System;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class EntityPropertyExtensions
    {
        public static EntityPropertyType GetPropertyType(this EntityPropertyElement propertyElement)
        {
            var typeFeature = propertyElement.Features.OfType<EntityPropertyTypeFeature>().FirstOrDefault();
            if (typeFeature == null)
            {
                throw new InvalidOperationException("The type was not specified.");
            }
            return typeFeature.PropertyType;
        }

        public static bool IsNullable(this EntityPropertyElement propertyElement)
        {
            var nullableFeature = propertyElement.Features.OfType<EntityPropertyNullableFeature>().FirstOrDefault();
            var isNullable = nullableFeature == null || nullableFeature.IsNullable;
            return isNullable;
        }
    }
}