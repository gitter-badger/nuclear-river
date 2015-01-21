using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class EntityElementExtensions
    {
        public static string ResolveFullName(this EntityElement entityElement)
        {
            if (entityElement == null)
            {
                throw new ArgumentNullException("entityElement");
            }

            return entityElement.Identity.ResolveFullName();
        }

        public static string ResolveName(this EntityElement entityElement)
        {
            if (entityElement == null)
            {
                throw new ArgumentNullException("entityElement");
            }

            return entityElement.Identity.ResolveName();
        }

        public static string ResolveName(this EntityElement entityElement, out string schema)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            schema = null;
            var name = entityElement.ResolveName();
            if (name != null)
            {
                var index = name.IndexOf('.');
                if (index >= 0)
                {
                    schema = name.Substring(0, index);
                    return name.Substring(index + 1);
                }
            }
            return name;
        }

        public static string GetEntitySetName(this EntityElement entityElement)
        {
            var collectionFeature = entityElement.Features.OfType<EntitySetFeature>().FirstOrDefault();
                return collectionFeature == null
                    ? null
                    : collectionFeature.EntitySetName;
        }

        public static IEnumerable<EntityPropertyElement> GetProperties(this EntityElement entityElement)
        {
            return entityElement.Elements.OfType<EntityPropertyElement>();
        }

        public static IEnumerable<EntityPropertyElement> GetKeyProperties(this EntityElement entityElement)
        {
            var identityFeature = entityElement.Features.OfType<EntityIdentityFeature>().FirstOrDefault();
            return identityFeature == null 
                ? Enumerable.Empty<EntityPropertyElement>() 
                : identityFeature.IdentifyingProperties;
        }

        public static IEnumerable<EntityRelationElement> GetRelations(this EntityElement entityElement)
        {
            return entityElement.Elements.OfType<EntityRelationElement>();
        }
    }
}