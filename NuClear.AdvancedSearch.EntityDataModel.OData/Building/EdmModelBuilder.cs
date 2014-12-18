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
//            var relations = (_source.Relations ?? Enumerable.Empty<EdmEntityRelationInfo>()).GroupBy(x => x.SourceEntity.Name).ToDictionary(x => x.Key, x => x.AsEnumerable());
//
            var model = new EdmModel();
            
            var container = new EdmEntityContainer(ns, "DefaultContainer");
            model.AddElement(container);

            foreach (var entityInfo in _source.Entities ?? Enumerable.Empty<EdmEntityType>())
            {
                var entity = BuildEntity(ns, entityInfo);
                model.AddElement(entity);
                if (/*entityInfo.IsOpen &&*/ (entity is IEdmEntityType))
                {
                    container.AddEntitySet(entityInfo.Name, (IEdmEntityType)entity);
                }

//                IEnumerable<EdmEntityRelationInfo> rels;
//                if (relations.TryGetValue(entityInfo.Name, out rels) && (entity is IEdmEntityType))
//                {
//                    foreach (var rel in rels)
//                    {
//                        ((EdmEntityType)entity).AddProperty(BuildNavigationProperty((EdmEntityType)entity, rel));
//                    }
//                }
            }

            return model;
        }

        #region Entity Building

        private static IEdmSchemaElement BuildEntity(string namespaceName, EdmEntityType entityType)
        {
            return entityType.HasKey ? BuildEntityType(namespaceName, entityType) : BuildComplexType(namespaceName, entityType);
        }

        private static IEdmSchemaElement BuildComplexType(string namespaceName, EdmEntityType entityType)
        {
            var entityType = new EdmComplexType(namespaceName, entityInfo.Name);

            foreach (var propertyInfo in entityInfo.Properties)
            {
                entityType.AddProperty(BuildProperty(entityType, propertyInfo));
            }

            return entityType;
        }

        private static IEdmSchemaElement BuildEntityType(string namespaceName, EdmEntityType entityType)
        {
            var entityType = new Microsoft.OData.Edm.Library.EdmEntityType(namespaceName, entityInfo.Name);

            foreach (var propertyInfo in entityInfo.Properties)
            {
                entityType.AddProperty(BuildProperty(entityType, propertyInfo));
            }

            if (entityInfo.HasKey)
            {
                var keys = new HashSet<string>(entityInfo.Keys.Select(x => x.Name));
                entityType.AddKeys(entityType.DeclaredProperties.OfType<IEdmStructuralProperty>().Where(x => keys.Contains(x.Name)));
            }

//            foreach (var relation in entityInfo.Relations)
//            {
//                entityType.AddProperty(BuildNavigationProperty(entityType, relation));
//            }

            return entityType;
        }

        #endregion

        #region Property Building

        private static IEdmProperty BuildProperty(EdmStructuredType entityType, EdmEntityPropertyInfo propertyInfo)
        {
            var property = new EdmStructuralProperty(entityType, propertyInfo.Name, ResolvePropertyType(propertyInfo.TypeReference));
            //var id = books.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int64, false);
            //books.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            return property;
        }

        private static IEdmTypeReference ResolvePropertyType(EdmTypeReference typeReference)
        {
            var primitiveType = typeReference.Type as EdmPrimitiveType;
            if (primitiveType != null)
            {
                return EdmCoreModel.Instance.GetPrimitive(ConvertPrimitiveType(primitiveType.TypeKind), typeReference.IsNullable);
            }

            var enumType = typeReference.Type as EdmEnumType;
            if (enumType != null)
            {
                var type = new Microsoft.OData.Edm.Library.EdmEnumType("MyNs", "TypeName");
                return new EdmEnumTypeReference(type, typeReference.IsNullable);
            }

            throw new NotSupportedException();
        }

        private static Microsoft.OData.Edm.EdmPrimitiveTypeKind ConvertPrimitiveType(EdmPrimitiveTypeKind typeKind)
        {
            switch (typeKind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Boolean;
                case EdmPrimitiveTypeKind.String:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.String;
                case EdmPrimitiveTypeKind.Int16:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Int16;
                case EdmPrimitiveTypeKind.Int32:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Int32;
                case EdmPrimitiveTypeKind.Int64:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Int64;
                case EdmPrimitiveTypeKind.Byte:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Byte;
                case EdmPrimitiveTypeKind.SByte:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.SByte;
                case EdmPrimitiveTypeKind.Single:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Single;
                case EdmPrimitiveTypeKind.Double:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Double;
                case EdmPrimitiveTypeKind.Decimal:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Decimal;
                case EdmPrimitiveTypeKind.Date:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Date;
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.DateTimeOffset;
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.TimeOfDay;
                case EdmPrimitiveTypeKind.Guid:
                    return Microsoft.OData.Edm.EdmPrimitiveTypeKind.Guid;
                default:
                    throw new ArgumentOutOfRangeException("typeKind");
            }
        }

        #endregion

        #region Navigation Property Building

        private static EdmNavigationProperty BuildNavigationProperty(IEdmEntityType entityType, EdmEntityRelationInfo relationInfo)
        {
            var info = new EdmNavigationPropertyInfo
                       {
                           Name = relationInfo.Name, 
                           Target = entityType, 
                           TargetMultiplicity = ConvertMultiplicity(relationInfo.TargetMultiplicity),
                           //ContainsTarget = 
                       };
            return EdmNavigationProperty.CreateNavigationProperty(entityType, info);
        }

        private static EdmMultiplicity ConvertMultiplicity(EdmEntityRelationMultiplicity targetMultiplicity)
        {
            switch (targetMultiplicity)
            {
                case EdmEntityRelationMultiplicity.ZeroOrOne:
                    return EdmMultiplicity.ZeroOrOne;
                case EdmEntityRelationMultiplicity.One:
                    return EdmMultiplicity.One;
                case EdmEntityRelationMultiplicity.Many:
                    return EdmMultiplicity.Many;
                default:
                    throw new ArgumentOutOfRangeException("targetMultiplicity");
            }
        }

        #endregion
    }
}
