using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmModelBuilder
    {
        private readonly IEdmModelSource _source;

        public EdmModelBuilder(IEdmModelSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            _source = source;
        }

        public IEdmModel Build()
        {
            var ns = _source.Namespace;

//
//            var container = new EdmEntityContainer(ns, "Container");
//            container.AddEntitySet("Books", books);

            var model = new EdmModel();

            foreach (var entityInfo in _source.Entities ?? Enumerable.Empty<EdmEntityInfo>())
            {
                model.AddElement(BuildEntity(ns, entityInfo));
            }

//            model.AddElement(container);
//            model.AddElement(books);

            return model;
        }

        private IEdmSchemaElement BuildEntity(string namespaceName, EdmEntityInfo entityInfo)
        {
            var entityType = entityInfo.HasKey
                ? (EdmStructuredType) new EdmEntityType(namespaceName, entityInfo.Name)
                : (EdmStructuredType) new EdmComplexType(namespaceName, entityInfo.Name);

            foreach (var propertyInfo in entityInfo.Properties)
            {
                var property = BuildProperty(entityType, propertyInfo);
                entityType.AddProperty(property);
            }

            if (entityInfo.HasKey)
            {
                var keys = new HashSet<string>(entityInfo.Keys.Select(x => x.Name));
                ((EdmEntityType)entityType).AddKeys(entityType.DeclaredProperties.OfType<IEdmStructuralProperty>().Where(x => keys.Contains(x.Name)));
            }

            return (IEdmSchemaElement)entityType;
        }

        private IEdmProperty BuildProperty(EdmStructuredType entityType, EdmEntityPropertyInfo propertyInfo)
        {
            var property = new EdmStructuralProperty(entityType, propertyInfo.Name, ResolvePropertyType(propertyInfo.Type, propertyInfo.IsNullable));
            //var id = books.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int64, false);
            //books.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            return property;
        }

        private static IEdmTypeReference ResolvePropertyType(EdmEntityPropertyType propertyType, bool isNullable)
        {
            return EdmCoreModel.Instance.GetPrimitive(ConvertPrimitiveType(propertyType.TypeKind), isNullable);
        }

        private static EdmPrimitiveTypeKind ConvertPrimitiveType(EdmEntityPropertyTypeKind typeKind)
        {
            switch (typeKind)
            {
                case EdmEntityPropertyTypeKind.Boolean:
                    return EdmPrimitiveTypeKind.Boolean;
                case EdmEntityPropertyTypeKind.String:
                    return EdmPrimitiveTypeKind.String;
                case EdmEntityPropertyTypeKind.Int16:
                    return EdmPrimitiveTypeKind.Int16;
                case EdmEntityPropertyTypeKind.Int32:
                    return EdmPrimitiveTypeKind.Int32;
                case EdmEntityPropertyTypeKind.Int64:
                    return EdmPrimitiveTypeKind.Int64;
                case EdmEntityPropertyTypeKind.Byte:
                    return EdmPrimitiveTypeKind.Byte;
                case EdmEntityPropertyTypeKind.SByte:
                    return EdmPrimitiveTypeKind.SByte;
                case EdmEntityPropertyTypeKind.Single:
                    return EdmPrimitiveTypeKind.Single;
                case EdmEntityPropertyTypeKind.Double:
                    return EdmPrimitiveTypeKind.Double;
                case EdmEntityPropertyTypeKind.Decimal:
                    return EdmPrimitiveTypeKind.Decimal;
                case EdmEntityPropertyTypeKind.Date:
                    return EdmPrimitiveTypeKind.Date;
                case EdmEntityPropertyTypeKind.DateTimeOffset:
                    return EdmPrimitiveTypeKind.DateTimeOffset;
                case EdmEntityPropertyTypeKind.TimeOfDay:
                    return EdmPrimitiveTypeKind.TimeOfDay;
                case EdmEntityPropertyTypeKind.Guid:
                    return EdmPrimitiveTypeKind.Guid;
                default:
                    throw new ArgumentOutOfRangeException("typeKind");
            }
        }

        private void CreateContainer()
        {
            
        }
    }
}
