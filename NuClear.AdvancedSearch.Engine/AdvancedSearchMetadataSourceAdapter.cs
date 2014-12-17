using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.Engine
{
    public sealed class AdvancedSearchMetadataSourceAdapter : IEdmModelSource
    {
        private readonly string _namespaceName;
        private readonly IReadOnlyCollection<EdmEntityInfo> _entities;

        public AdvancedSearchMetadataSourceAdapter(BoundedContextElement boundedContext)
        {
            if (boundedContext == null)
            {
                throw new ArgumentNullException("boundedContext");
            }

            // TODO {s.pomadin, 16.12.2014}: provide a proper solution
            _namespaceName = boundedContext.Identity.Id.ToString().Replace("erm://metadata/", "").Replace("/", ".");

            _entities = boundedContext.Elements.OfType<EntityElement>().Select(Convert).ToList();
        }

        public string Namespace
        {
            get
            {
                return _namespaceName;
            }
        }

        public IReadOnlyCollection<EdmEntityInfo> Entities
        {
            get
            {
                return _entities;
            }
        }

        private static EdmEntityInfo Convert(EntityElement entityElement)
        {
            var name = ResolveName(entityElement.Identity);
            var properties = entityElement.Properties.ToDictionary(x => x.Identity, Convert);
            var keys = entityElement.Keys.Select(x => properties[x.Identity]).ToArray();
            
            var entityInfo = new EdmEntityInfo(name, properties.Values, keys);
            return entityInfo;
        }

        private static EdmEntityPropertyInfo Convert(EntityPropertyElement propertyElement)
        {
            var name = ResolveName(propertyElement.Identity);
            var type = new EdmEntityPropertyType(Convert(propertyElement.PropertyType));

            var propertyInfo = new EdmEntityPropertyInfo(name, type, propertyElement.IsNullable);

            return propertyInfo;
        }

        private static EdmEntityPropertyTypeKind Convert(EntityPropertyType propertyType)
        {
            switch (propertyType)
            {
                case EntityPropertyType.Boolean:
                    return EdmEntityPropertyTypeKind.Boolean;
                
                case EntityPropertyType.Byte:
                    return EdmEntityPropertyTypeKind.Byte;
                case EntityPropertyType.SByte:
                    return EdmEntityPropertyTypeKind.SByte;
                case EntityPropertyType.Int16:
                    return EdmEntityPropertyTypeKind.Int16;
                case EntityPropertyType.Int32:
                    return EdmEntityPropertyTypeKind.Int32;
                case EntityPropertyType.Int64:
                    return EdmEntityPropertyTypeKind.Int64;
                
                case EntityPropertyType.Single:
                    return EdmEntityPropertyTypeKind.Single;
                case EntityPropertyType.Double:
                    return EdmEntityPropertyTypeKind.Double;
                case EntityPropertyType.Decimal:
                    return EdmEntityPropertyTypeKind.Decimal;

                case EntityPropertyType.DateTime:
                    return EdmEntityPropertyTypeKind.Date;

                case EntityPropertyType.String:
                    return EdmEntityPropertyTypeKind.String;

                default:
                    throw new ArgumentOutOfRangeException("propertyType");
            }
        }

        private static string ResolveName(IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.ToString().Split('/').LastOrDefault();
        }
    }
}