using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public static class EdmModelBuilder
    {
        private const string DefaultContainName = "DefaultContainer";

        public static IEdmModel Build(BoundedContextElement context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return BuildModel(ResolveNamespaceName(context.Identity), context.Entities);
        }

        private static IEdmModel BuildModel(string namespaceName, IEnumerable<EntityElement> entities)
        {
            var typeBuilder = new TypeBuilder(namespaceName);

            var model = new EdmModel();

            model.AddElement(BuildContainer(entities, typeBuilder));
            
            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
                model.AddElement(registeredType);
            }

            return model;
        }

        private static IEdmEntityContainer BuildContainer(IEnumerable<EntityElement> entities, TypeBuilder typeBuilder)
        {
            var container = new EdmEntityContainer(typeBuilder.NamespaceName, DefaultContainName);

            foreach (var entityElement in entities)
            {
                var entitySetName = ResolveName(entityElement.Identity);
                var entityType = typeBuilder.ResolveEntityType(entityElement);

                container.AddEntitySet(entitySetName, entityType);
            }

            return container;
        }

        #region Utils

        private static string ResolveNamespaceName(IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Replace("/", ".");
        }

        private static string ResolveName(IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/').LastOrDefault();
        }

        #endregion

        #region TypeBuilder

        private class TypeBuilder
        {
            private readonly Dictionary<string, IEdmSchemaType> _registeredTypes;

            public TypeBuilder(string namespaceName)
            {
                NamespaceName = namespaceName;
                _registeredTypes = new Dictionary<string, IEdmSchemaType>();
            }

            public string NamespaceName { get; private set; }

            public IEnumerable<IEdmSchemaType> RegisteredTypes
            {
                get
                {
                    return _registeredTypes.Values;
                }
            }

            public IEdmEntityType ResolveEntityType(EntityElement entityElement)
            {
                var typeName = ResolveName(entityElement.Identity);

                IEdmSchemaType entityType;
                if (!_registeredTypes.TryGetValue(typeName, out entityType))
                {
                    _registeredTypes.Add(typeName, entityType = BuildEntityType(typeName, entityElement));
                }

                return (IEdmEntityType)entityType;
            }

            private IEdmComplexType ResolveComplexType(EntityElement entityElement)
            {
                var typeName = ResolveName(entityElement.Identity);

                IEdmSchemaType complexType;
                if (!_registeredTypes.TryGetValue(typeName, out complexType))
                {
                    _registeredTypes.Add(typeName, complexType = BuildComplexType(typeName, entityElement));
                }

                return (IEdmComplexType)complexType;
            }

            private IEdmEnumType ResolveEnumType(EntityPropertyEnumTypeFeature feature)
            {
                var typeName = feature.Name;

                IEdmSchemaType enumType;
                if (!_registeredTypes.TryGetValue(typeName, out enumType))
                {
                    _registeredTypes.Add(typeName, enumType = BuildEnumType(typeName, feature));
                }

                return (IEdmEnumType)enumType;
            }

            private IEdmTypeReference ResolveTypeReference(EntityPropertyElement propertyElement)
            {
                if (IsPrimitiveType(propertyElement.PropertyType))
                {
                    return EdmCoreModel.Instance.GetPrimitive(Convert(propertyElement.PropertyType), propertyElement.IsNullable);
                }

                if (propertyElement.PropertyType == EntityPropertyType.Enum)
                {
                    var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
                    if (feature == null)
                    {
                        throw new ArgumentException("The enum type was not completely specified.");
                    }

                    return new EdmEnumTypeReference(ResolveEnumType(feature), propertyElement.IsNullable);
                }

                throw new NotSupportedException();
            }

            private IEdmTypeReference ResolveTypeReference(EntityRelationElement relationElement)
            {
                var complexType = ResolveComplexType(relationElement.Target);

                IEdmTypeReference typeReference;
                switch (relationElement.Cardinality)
                {
                    case EntityRelationCardinality.One:
                        typeReference = new EdmComplexTypeReference(complexType, false);
                        break;
                    case EntityRelationCardinality.OptionalOne:
                        typeReference = new EdmComplexTypeReference(complexType, true);
                        break;
                    case EntityRelationCardinality.Many:
                        typeReference = EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, true));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return typeReference;
            }

            private IEdmEntityType BuildEntityType(string typeName, EntityElement entityElement)
            {
                var entityType = new EdmEntityType(NamespaceName, typeName);
                var keyIds = new HashSet<IMetadataElementIdentity>(entityElement.Keys.Select(x => x.Identity));

                foreach (var propertyElement in entityElement.Properties)
                {
                    var propertyName = ResolveName(propertyElement.Identity);
                    var typeReference = ResolveTypeReference(propertyElement);

                    var property = entityType.AddStructuralProperty(propertyName, typeReference);
                    if (keyIds.Contains(propertyElement.Identity))
                    {
                        entityType.AddKeys(property);
                    }
                }

                foreach (var relationElement in entityElement.Relations)
                {
                    var propertyName = ResolveName(relationElement.Identity);
                    var typeReference = ResolveTypeReference(relationElement);

                    entityType.AddStructuralProperty(propertyName, typeReference);
                }

                return entityType;
            }

            private IEdmComplexType BuildComplexType(string typeName, EntityElement entityElement)
            {
                var entityType = new EdmComplexType(NamespaceName, typeName);

                foreach (var propertyElement in entityElement.Properties)
                {
                    var propertyName = ResolveName(propertyElement.Identity);
                    var typeReference = ResolveTypeReference(propertyElement);

                    entityType.AddStructuralProperty(propertyName, typeReference);
                }

                return entityType;
            }

            private IEdmEnumType BuildEnumType(string typeName, EntityPropertyEnumTypeFeature feature)
            {
                var enumType = new EdmEnumType(NamespaceName, typeName, Convert(feature.UnderlyingType), false);

                foreach (var member in feature.Members)
                {
                    enumType.AddMember(member.Key, new EdmIntegerConstant(member.Value));
                }

                return enumType;
            }

            private static bool IsPrimitiveType(EntityPropertyType propertyType)
            {
                return propertyType != EntityPropertyType.Enum;
            }

            private static EdmPrimitiveTypeKind Convert(EntityPropertyType propertyType)
            {
                switch (propertyType)
                {
                    case EntityPropertyType.Boolean:
                        return EdmPrimitiveTypeKind.Boolean;

                    case EntityPropertyType.Byte:
                        return EdmPrimitiveTypeKind.Byte;
                    case EntityPropertyType.SByte:
                        return EdmPrimitiveTypeKind.SByte;
                    case EntityPropertyType.Int16:
                        return EdmPrimitiveTypeKind.Int16;
                    case EntityPropertyType.Int32:
                        return EdmPrimitiveTypeKind.Int32;
                    case EntityPropertyType.Int64:
                        return EdmPrimitiveTypeKind.Int64;

                    case EntityPropertyType.Single:
                        return EdmPrimitiveTypeKind.Single;
                    case EntityPropertyType.Double:
                        return EdmPrimitiveTypeKind.Double;
                    case EntityPropertyType.Decimal:
                        return EdmPrimitiveTypeKind.Decimal;

                    case EntityPropertyType.DateTime:
                        return EdmPrimitiveTypeKind.Date;

                    case EntityPropertyType.String:
                        return EdmPrimitiveTypeKind.String;

                    default:
                        throw new ArgumentOutOfRangeException("propertyType");
                }
            }
        }

        #endregion
    }
}
