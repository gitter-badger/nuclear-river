using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class EntityPropertyExtensions
    {
        public static string ResolveName(this EntityPropertyElement propertyElement)
        {
            if (propertyElement == null)
            {
                throw new ArgumentNullException("propertyElement");
            }

            return propertyElement.Identity.ResolveName();
        }

        public static EntityPropertyType GetPropertyType(this EntityPropertyElement propertyElement)
        {
            var typeFeature = propertyElement.Features.OfType<EntityPropertyTypeFeature>().FirstOrDefault();
            if (typeFeature == null)
            {
                throw new InvalidOperationException("The type was not specified.");
            }

            return typeFeature.PropertyType;
        }

        public static string GetEnumName(this EntityPropertyElement propertyElement)
        {
            if (propertyElement.GetPropertyType() != EntityPropertyType.Enum)
            {
                throw new ArgumentException("The specified element is not of an enum type.");
            }

            var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
            if (feature != null)
            {
                return feature.Name;
            }
            
            throw new NotSupportedException();
        }

        public static EntityPropertyType GetUnderlyingPropertyType(this EntityPropertyElement propertyElement)
        {
            if (propertyElement.GetPropertyType() != EntityPropertyType.Enum)
            {
                throw new ArgumentException("The specified element is not of an enum type.");
            }

            var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
            if (feature != null)
            {
                return feature.UnderlyingType;
            }
            
            throw new NotSupportedException();
        }

        public static IReadOnlyDictionary<string, long> GetEnumMembers(this EntityPropertyElement propertyElement)
        {
            if (propertyElement.GetPropertyType() != EntityPropertyType.Enum)
            {
                throw new ArgumentException("The specified element is not of an enum type.");
            }

            var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
            if (feature != null)
            {
                return feature.Members;
            }
            
            throw new NotSupportedException();
        }

        public static bool IsNullable(this EntityPropertyElement propertyElement)
        {
            var nullableFeature = propertyElement.Features.OfType<EntityPropertyNullableFeature>().FirstOrDefault();
            var isNullable = nullableFeature != null && nullableFeature.IsNullable;
            return isNullable;
        }
    }
}