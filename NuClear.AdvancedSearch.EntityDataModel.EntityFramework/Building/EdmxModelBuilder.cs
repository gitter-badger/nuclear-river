using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;

namespace NuClear.EntityDataModel.EntityFramework.Building
{
    public static class EdmxModelBuilder
    {
        private static readonly DbProviderInfo DefaultProviderInfo = new DbProviderInfo("System.Data.SqlClient", "2012");

        public static DbModel Build(IMetadataProvider metadataProvider, Uri uri)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException("metadataProvider");
            }

            BoundedContextElement context;
            metadataProvider.TryGetMetadata(uri, out context);

            var model = Build(context);

            BuildStoreModel(model, context.Entities);
            BuildOneToOneMapping(model);

            return model;
        }

        public static DbModel Build(BoundedContextElement context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return BuildEdmModel(ResolveNamespaceName(context.Identity), context.Entities);
        }

        private static DbModel BuildEdmModel(string namespaceName, IEnumerable<EntityElement> entities)
        {
            var typeBuilder = new EdmTypeBuilder(namespaceName);

            var builder = new DbModelBuilder();
            
            var model = builder.Build(DefaultProviderInfo);

            foreach (var entityElement in entities)
            {
                var entitySetName = entityElement.GetCollectionName() ?? ResolveName(entityElement.Identity);
                var entityType = typeBuilder.ResolveComplexType(entityElement);

                model.ConceptualModel.Container.AddEntitySetBase(EntitySet.Create(entitySetName, typeBuilder.NamespaceName, entityType.Name, null, entityType, new MetadataProperty[0]));
            }

            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
                var entityType = registeredType as EntityType;
                if (entityType != null)
                {
                    model.ConceptualModel.AddItem(entityType);
                }
                var enumType = registeredType as EnumType;
                if (enumType != null)
                {
                    model.ConceptualModel.AddItem(enumType);
                }
            }

            return model;
        }

        private static void BuildStoreModel(DbModel model, IEnumerable<EntityElement> entities)
        {
            var typeBuilder = new StoreTypeBuilder(model.ProviderManifest);

            foreach (var entityElement in entities)
            {
                var entitySetName = entityElement.GetCollectionName() ?? ResolveName(entityElement.Identity);
                var entityType = typeBuilder.ResolveComplexType(entityElement);

                model.StoreModel.Container.AddEntitySetBase(EntitySet.Create(entitySetName, typeBuilder.NamespaceName, entityType.Name, null, entityType, new MetadataProperty[0]));
            }

            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
                var entityType = registeredType as EntityType;
                if (entityType != null)
                {
                    model.StoreModel.AddItem(entityType);
                }
                var enumType = registeredType as EnumType;
                if (enumType != null)
                {
                    model.StoreModel.AddItem(enumType);
                }
            }
        }

        private static void BuildOneToOneMapping(DbModel model)
        {
            foreach (var entitySet in model.ConceptualModel.Container.EntitySets)
            {
                var mapping = new EntitySetMapping(entitySet, model.ConceptualToStoreMapping);

                var entityTypeMapping = new EntityTypeMapping(mapping);
                entityTypeMapping.AddType(entitySet.ElementType);

                var storeEntitySet = model.StoreModel.Container.EntitySets.Single(x => x.Name == entitySet.Name);
                
                var mappingFragment = new MappingFragment(storeEntitySet, entityTypeMapping, false);

                foreach (var property in entitySet.ElementType.DeclaredProperties)
                {
                    mappingFragment.AddPropertyMapping(new ScalarPropertyMapping(property, storeEntitySet.ElementType.Properties.Single(x => x.Name == property.Name)));
                }
                
                entityTypeMapping.AddFragment(mappingFragment);

                mapping.AddTypeMapping(entityTypeMapping);

                model.ConceptualToStoreMapping.AddSetMapping(mapping);
            }
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

        #region EdmTypeBuilder

        private class EdmTypeBuilder
        {
            private readonly Dictionary<string, EdmType> _registeredTypes;

            public EdmTypeBuilder(string namespaceName)
            {
                NamespaceName = namespaceName;
                _registeredTypes = new Dictionary<string, EdmType>();
            }

            public string NamespaceName { get; private set; }

            public IEnumerable<EdmType> RegisteredTypes
            {
                get
                {
                    return _registeredTypes.Values;
                }
            }

            public EntityType ResolveComplexType(EntityElement entityElement)
            {
                var typeName = ResolveName(entityElement.Identity);

                EdmType complexType;
                if (!_registeredTypes.TryGetValue(typeName, out complexType))
                {
                    _registeredTypes.Add(typeName,
                        complexType = entityElement.GetKeyProperties().Any()
//                        ? (IEdmSchemaType)BuildEntityType(typeName, entityElement)
//                        : (IEdmSchemaType)BuildComplexType(typeName, entityElement));
                        ? (EntityType)BuildEntityType(typeName, entityElement)
                        : (EntityType)BuildEntityType(typeName, entityElement));
                }

                return (EntityType)complexType;
            }

            private EnumType ResolveEnumType(EntityPropertyElement propertyElement)
            {
                var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
                if (feature == null)
                {
                    throw new ArgumentException("The enum type was not completely specified.");
                }

                var typeName = feature.Name;

                EdmType enumType;
                if (!_registeredTypes.TryGetValue(typeName, out enumType))
                {
                    _registeredTypes.Add(typeName, enumType = BuildEnumType(typeName, feature));
                }

                return (EnumType)enumType;
            }

//            private IEdmTypeReference ResolveTypeReference(EntityRelationElement relationElement)
//            {
//                var complexType = (IEdmComplexType)ResolveComplexType(relationElement.GetTarget());
//
//                IEdmTypeReference typeReference;
//                switch (relationElement.GetCardinality())
//                {
//                    case EntityRelationCardinality.One:
//                        typeReference = new EdmComplexTypeReference(complexType, false);
//                        break;
//                    case EntityRelationCardinality.OptionalOne:
//                        typeReference = new EdmComplexTypeReference(complexType, true);
//                        break;
//                    case EntityRelationCardinality.Many:
//                        typeReference = EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, true));
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//
//                return typeReference;
//            }

            private EntityType BuildEntityType(string typeName, EntityElement entityElement)
            {
                var keyIds = new HashSet<IMetadataElementIdentity>(entityElement.GetKeyProperties().Select(x => x.Identity));

                var keyNames = new List<string>();
                var properties = new List<EdmProperty>();

                foreach (var propertyElement in entityElement.GetProperties())
                {
                    var propertyName = ResolveName(propertyElement.Identity);
                    var propertyType = propertyElement.GetPropertyType();

                    EdmProperty property;
                    if (IsPrimitiveType(propertyType))
                    {
                        property = EdmProperty.CreatePrimitive(propertyName, PrimitiveType.GetEdmPrimitiveType(Convert(propertyType)));
                    }
                    else if (propertyType == EntityPropertyType.Enum)
                    {
                        property = EdmProperty.CreateEnum(propertyName, ResolveEnumType(propertyElement));
                    }
                    else
                    {
                        //var typeReference = ResolveTypeReference(propertyElement);
                        throw new NotImplementedException();
                    }
                    
                    property.Nullable = propertyElement.IsNullable();
                    properties.Add(property);

                    if (keyIds.Contains(propertyElement.Identity))
                    {
                        keyNames.Add(propertyName);
                    }
                }

                //                foreach (var relationElement in entityElement.GetRelations())
                //                {
                //                    var propertyName = ResolveName(relationElement.Identity);
                //                    var structuredType = ResolveComplexType(relationElement.GetTarget());
                //
                //                    if (structuredType is IEdmComplexType)
                //                    {
                //                        var typeReference = ResolveTypeReference(relationElement);
                //                        entityType.AddStructuralProperty(propertyName, typeReference);
                //                    }
                //
                //                    var relatedEntityType = structuredType as IEdmEntityType;
                //                    if (relatedEntityType != null)
                //                    {
                //
                //                        entityType.AddUnidirectionalNavigation(
                //                                                               new EdmNavigationPropertyInfo
                //                                                               {
                //                                                                   Name = propertyName,
                //                                                                   Target = relatedEntityType,
                //                                                                   TargetMultiplicity = Convert(relationElement.GetCardinality())
                //                                                               });
                //                    }
                //                }


                return EntityType.Create(typeName, NamespaceName, DataSpace.CSpace,keyNames, properties, new MetadataProperty[0]);
            }

//            private IEdmComplexType BuildComplexType(string typeName, EntityElement entityElement)
//            {
//                var entityType = new EdmComplexType(NamespaceName, typeName);
//
//                foreach (var propertyElement in entityElement.GetProperties())
//                {
//                    var propertyName = ResolveName(propertyElement.Identity);
//                    var typeReference = ResolveTypeReference(propertyElement);
//
//                    entityType.AddStructuralProperty(propertyName, typeReference);
//                }
//
//                foreach (var relationElement in entityElement.GetRelations())
//                {
//                    var propertyName = ResolveName(relationElement.Identity);
//                    var structuredType = ResolveComplexType(relationElement.GetTarget());
//
//                    if (structuredType is IEdmComplexType)
//                    {
//                        var typeReference = ResolveTypeReference(relationElement);
//                        entityType.AddStructuralProperty(propertyName, typeReference);
//                    }
//                }
//
//                return entityType;
//            }

            private EnumType BuildEnumType(string typeName, EntityPropertyEnumTypeFeature feature)
            {
                return EnumType.Create(
                    typeName, NamespaceName, 
                    PrimitiveType.GetEdmPrimitiveType(Convert(feature.UnderlyingType)), 
                    false, 
                    feature.Members.Select(memberPair => EnumMember.Create(memberPair.Key, memberPair.Value, new MetadataProperty[0])), 
                    new MetadataProperty[0]);
            }

            private static bool IsPrimitiveType(EntityPropertyType propertyType)
            {
                return propertyType != EntityPropertyType.Enum;
            }

            private static PrimitiveTypeKind Convert(EntityPropertyType propertyType)
            {
                switch (propertyType)
                {
                    case EntityPropertyType.Boolean:
                        return PrimitiveTypeKind.Boolean;

                    case EntityPropertyType.Byte:
                        return PrimitiveTypeKind.Byte;
                    case EntityPropertyType.SByte:
                        return PrimitiveTypeKind.SByte;
                    case EntityPropertyType.Int16:
                        return PrimitiveTypeKind.Int16;
                    case EntityPropertyType.Int32:
                        return PrimitiveTypeKind.Int32;
                    case EntityPropertyType.Int64:
                        return PrimitiveTypeKind.Int64;

                    case EntityPropertyType.Single:
                        return PrimitiveTypeKind.Single;
                    case EntityPropertyType.Double:
                        return PrimitiveTypeKind.Double;
                    case EntityPropertyType.Decimal:
                        return PrimitiveTypeKind.Decimal;

                    case EntityPropertyType.DateTime:
                        return PrimitiveTypeKind.DateTime;

                    case EntityPropertyType.String:
                        return PrimitiveTypeKind.String;

                    default:
                        throw new ArgumentOutOfRangeException("propertyType");
                }
            }

//            private static EdmMultiplicity Convert(EntityRelationCardinality cardinality)
//            {
//                switch (cardinality)
//                {
//                    case EntityRelationCardinality.One:
//                        return EdmMultiplicity.One;
//                    case EntityRelationCardinality.OptionalOne:
//                        return EdmMultiplicity.ZeroOrOne;
//                    case EntityRelationCardinality.Many:
//                        return EdmMultiplicity.Many;
//                    default:
//                        throw new ArgumentOutOfRangeException("cardinality");
//                }
//            }
        }

        #endregion

        #region StoreTypeBuilder

        private class StoreTypeBuilder
        {
            private readonly DbProviderManifest _providerManifest;
            private readonly Dictionary<string, EdmType> _registeredTypes;

            public StoreTypeBuilder(DbProviderManifest providerManifest)
            {
                NamespaceName = "StoreSpace";
                _providerManifest = providerManifest;
                _registeredTypes = new Dictionary<string, EdmType>();
            }

            public string NamespaceName { get; private set; }

            public IEnumerable<EdmType> RegisteredTypes
            {
                get
                {
                    return _registeredTypes.Values;
                }
            }

            public EntityType ResolveComplexType(EntityElement entityElement)
            {
                var typeName = ResolveName(entityElement.Identity);

                EdmType complexType;
                if (!_registeredTypes.TryGetValue(typeName, out complexType))
                {
                    _registeredTypes.Add(typeName,
                        complexType = entityElement.GetKeyProperties().Any()
                        //                        ? (IEdmSchemaType)BuildEntityType(typeName, entityElement)
                        //                        : (IEdmSchemaType)BuildComplexType(typeName, entityElement));
                        ? (EntityType)BuildEntityType(typeName, entityElement)
                        : (EntityType)BuildEntityType(typeName, entityElement));
                }

                return (EntityType)complexType;
            }

            private EnumType ResolveEnumType(EntityPropertyElement propertyElement)
            {
                var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
                if (feature == null)
                {
                    throw new ArgumentException("The enum type was not completely specified.");
                }

                var typeName = feature.Name;

                EdmType enumType;
                if (!_registeredTypes.TryGetValue(typeName, out enumType))
                {
                    _registeredTypes.Add(typeName, enumType = BuildEnumType(typeName, feature));
                }

                return (EnumType)enumType;
            }

            //            private IEdmTypeReference ResolveTypeReference(EntityRelationElement relationElement)
            //            {
            //                var complexType = (IEdmComplexType)ResolveComplexType(relationElement.GetTarget());
            //
            //                IEdmTypeReference typeReference;
            //                switch (relationElement.GetCardinality())
            //                {
            //                    case EntityRelationCardinality.One:
            //                        typeReference = new EdmComplexTypeReference(complexType, false);
            //                        break;
            //                    case EntityRelationCardinality.OptionalOne:
            //                        typeReference = new EdmComplexTypeReference(complexType, true);
            //                        break;
            //                    case EntityRelationCardinality.Many:
            //                        typeReference = EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, true));
            //                        break;
            //                    default:
            //                        throw new ArgumentOutOfRangeException();
            //                }
            //
            //                return typeReference;
            //            }

            private EntityType BuildEntityType(string typeName, EntityElement entityElement)
            {
                var keyIds = new HashSet<IMetadataElementIdentity>(entityElement.GetKeyProperties().Select(x => x.Identity));

                var keyNames = new List<string>();
                var properties = new List<EdmProperty>();

                foreach (var propertyElement in entityElement.GetProperties())
                {
                    var propertyName = ResolveName(propertyElement.Identity);
                    var propertyType = propertyElement.GetPropertyType();

                    EdmProperty property;
                    if (IsPrimitiveType(propertyType))
                    {
                        var edmProperty = EdmProperty.CreatePrimitive(propertyName, PrimitiveType.GetEdmPrimitiveType(Convert(propertyType)));
                        property = EdmProperty.Create(propertyName, _providerManifest.GetStoreType(edmProperty.TypeUsage));
                    }
                    else if (propertyType == EntityPropertyType.Enum)
                    {
                        property = EdmProperty.CreateEnum(propertyName, ResolveEnumType(propertyElement));
                    }
                    else
                    {
                        //var typeReference = ResolveTypeReference(propertyElement);
                        throw new NotImplementedException();
                    }

                    property.Nullable = propertyElement.IsNullable();
                    properties.Add(property);

                    if (keyIds.Contains(propertyElement.Identity))
                    {
                        keyNames.Add(propertyName);
                    }
                }

                //                foreach (var relationElement in entityElement.GetRelations())
                //                {
                //                    var propertyName = ResolveName(relationElement.Identity);
                //                    var structuredType = ResolveComplexType(relationElement.GetTarget());
                //
                //                    if (structuredType is IEdmComplexType)
                //                    {
                //                        var typeReference = ResolveTypeReference(relationElement);
                //                        entityType.AddStructuralProperty(propertyName, typeReference);
                //                    }
                //
                //                    var relatedEntityType = structuredType as IEdmEntityType;
                //                    if (relatedEntityType != null)
                //                    {
                //
                //                        entityType.AddUnidirectionalNavigation(
                //                                                               new EdmNavigationPropertyInfo
                //                                                               {
                //                                                                   Name = propertyName,
                //                                                                   Target = relatedEntityType,
                //                                                                   TargetMultiplicity = Convert(relationElement.GetCardinality())
                //                                                               });
                //                    }
                //                }


                return EntityType.Create(typeName, NamespaceName, DataSpace.SSpace, keyNames, properties, new MetadataProperty[0]);
            }

            //            private IEdmComplexType BuildComplexType(string typeName, EntityElement entityElement)
            //            {
            //                var entityType = new EdmComplexType(NamespaceName, typeName);
            //
            //                foreach (var propertyElement in entityElement.GetProperties())
            //                {
            //                    var propertyName = ResolveName(propertyElement.Identity);
            //                    var typeReference = ResolveTypeReference(propertyElement);
            //
            //                    entityType.AddStructuralProperty(propertyName, typeReference);
            //                }
            //
            //                foreach (var relationElement in entityElement.GetRelations())
            //                {
            //                    var propertyName = ResolveName(relationElement.Identity);
            //                    var structuredType = ResolveComplexType(relationElement.GetTarget());
            //
            //                    if (structuredType is IEdmComplexType)
            //                    {
            //                        var typeReference = ResolveTypeReference(relationElement);
            //                        entityType.AddStructuralProperty(propertyName, typeReference);
            //                    }
            //                }
            //
            //                return entityType;
            //            }

            private EnumType BuildEnumType(string typeName, EntityPropertyEnumTypeFeature feature)
            {
                return EnumType.Create(
                    typeName, NamespaceName,
                    PrimitiveType.GetEdmPrimitiveType(Convert(feature.UnderlyingType)),
                    false,
                    feature.Members.Select(memberPair => EnumMember.Create(memberPair.Key, memberPair.Value, new MetadataProperty[0])),
                    new MetadataProperty[0]);
            }

            private static bool IsPrimitiveType(EntityPropertyType propertyType)
            {
                return propertyType != EntityPropertyType.Enum;
            }

            private static PrimitiveTypeKind Convert(EntityPropertyType propertyType)
            {
                switch (propertyType)
                {
                    case EntityPropertyType.Boolean:
                        return PrimitiveTypeKind.Boolean;

                    case EntityPropertyType.Byte:
                        return PrimitiveTypeKind.Byte;
                    case EntityPropertyType.SByte:
                        return PrimitiveTypeKind.SByte;
                    case EntityPropertyType.Int16:
                        return PrimitiveTypeKind.Int16;
                    case EntityPropertyType.Int32:
                        return PrimitiveTypeKind.Int32;
                    case EntityPropertyType.Int64:
                        return PrimitiveTypeKind.Int64;

                    case EntityPropertyType.Single:
                        return PrimitiveTypeKind.Single;
                    case EntityPropertyType.Double:
                        return PrimitiveTypeKind.Double;
                    case EntityPropertyType.Decimal:
                        return PrimitiveTypeKind.Decimal;

                    case EntityPropertyType.DateTime:
                        return PrimitiveTypeKind.DateTime;

                    case EntityPropertyType.String:
                        return PrimitiveTypeKind.String;

                    default:
                        throw new ArgumentOutOfRangeException("propertyType");
                }
            }

            //            private static EdmMultiplicity Convert(EntityRelationCardinality cardinality)
            //            {
            //                switch (cardinality)
            //                {
            //                    case EntityRelationCardinality.One:
            //                        return EdmMultiplicity.One;
            //                    case EntityRelationCardinality.OptionalOne:
            //                        return EdmMultiplicity.ZeroOrOne;
            //                    case EntityRelationCardinality.Many:
            //                        return EdmMultiplicity.Many;
            //                    default:
            //                        throw new ArgumentOutOfRangeException("cardinality");
            //                }
            //            }
        }

        #endregion
    }
}
