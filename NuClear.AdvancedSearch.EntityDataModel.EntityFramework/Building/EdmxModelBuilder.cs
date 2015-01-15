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
        private static readonly IEnumerable<MetadataProperty> EmptyMetadata = new MetadataProperty[0];

        public static DbModel Build(IMetadataProvider metadataProvider, Uri uri)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException("metadataProvider");
            }

            BoundedContextElement context;
            metadataProvider.TryGetMetadata(uri, out context);

            return Build(context);
        }

        public static DbModel Build(BoundedContextElement context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var builder = new DbModelBuilder();
            var model = builder.Build(DefaultProviderInfo);

            var cmNamespaceName = ResolveNamespaceName(context.Identity);
            var smNamespaceName = ResolveNamespaceName(context.Identity) + ".Store"; // FIXME {s.pomadin, 13.01.2015}: has to be improved

            BuildEdmModel(model.ConceptualModel, context.ConceptualModel, cmNamespaceName);
            BuildStoreModel(model.StoreModel, context.StoreModel, smNamespaceName, model.ProviderManifest);
            BuildOneToOneMapping(model);

            return model;
        }

        private static void BuildEdmModel(EdmModel model, StructuralModelElement conceptualModel, string namespaceName)
        {
            if (conceptualModel == null)
            {
                return;
            }

            var typeBuilder = new EdmTypeBuilder(namespaceName);

            foreach (var entityElement in conceptualModel.Entities)
            {
                var entitySetName = entityElement.GetEntitySetName() ?? ResolveName(entityElement.Identity);
                var entityType = typeBuilder.ResolveComplexType(entityElement);

                //model.Container.AddEntitySetBase(EntitySet.Create(entitySetName, typeBuilder.NamespaceName, entityType.Name, null, entityType, new MetadataProperty[0]));
                //model.Container.AddEntitySetBase(EntitySet.Create(entitySetName, null, null, null, entityType, new MetadataProperty[0]));
            }

            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
                var entityType = registeredType as EntityType;
                if (entityType != null)
                {
                    model.Container.AddEntitySetBase(EntitySet.Create(entityType.Name, null, null, null, entityType, EmptyMetadata));
                    model.AddItem(entityType);
                }
                var enumType = registeredType as EnumType;
                if (enumType != null)
                {
                    model.AddItem(enumType);
                }
            }

            foreach (var associationType in typeBuilder.AssociationTypes)
            {
                //var sourceType = associationType.
                var sourceType = ((AssociationEndMember)associationType.KeyMembers.First()).GetEntityType();
                var targetType = ((AssociationEndMember)associationType.KeyMembers.ElementAt(1)).GetEntityType();

                var sourceEntitySet = model.Container.EntitySets.FirstOrDefault(x => x.ElementType == sourceType);
                var targetEntitySet = model.Container.EntitySets.FirstOrDefault(x => x.ElementType == targetType);

                model.Container.AddEntitySetBase(AssociationSet.Create(associationType.Name, associationType, sourceEntitySet, targetEntitySet, EmptyMetadata));
                model.AddItem(associationType);
            }
        }

        private static void BuildStoreModel(EdmModel model, StructuralModelElement storeModel, string namespaceName, DbProviderManifest providerManifest)
        {
            if (storeModel == null)
            {
                return;
            }

            var typeBuilder = new StoreTypeBuilder(namespaceName, providerManifest);

            foreach (var entityElement in storeModel.Entities)
            {
                string schemaName;
                var tableName = ResolveName(entityElement.Identity, out schemaName);
                var entityName = entityElement.GetEntitySetName() ?? tableName;

                var entityType = typeBuilder.ResolveType(entityElement);
                var entitySet = EntitySet.Create(entityName, schemaName, tableName, null, entityType, new MetadataProperty[0]);

                model.Container.AddEntitySetBase(entitySet);
            }

            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
                var entityType = registeredType as EntityType;
                if (entityType != null)
                {
                    model.AddItem(entityType);
                }
                var enumType = registeredType as EnumType;
                if (enumType != null)
                {
                    model.AddItem(enumType);
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

                var storeEntitySet = model.StoreModel.Container.EntitySets.SingleOrDefault(x => x.Name.EndsWith(entitySet.Name));
                if (storeEntitySet == null) continue;
                
                var mappingFragment = new MappingFragment(storeEntitySet, entityTypeMapping, false);

                foreach (var property in entitySet.ElementType.DeclaredProperties)
                {
                    var targetProperty = storeEntitySet.ElementType.Properties.SingleOrDefault(x => x.Name == property.Name);
                    if (targetProperty != null)
                    {
                        mappingFragment.AddPropertyMapping(new ScalarPropertyMapping(property, targetProperty));
                    }
                }
                
                entityTypeMapping.AddFragment(mappingFragment);

                mapping.AddTypeMapping(entityTypeMapping);

                model.ConceptualToStoreMapping.AddSetMapping(mapping);
            }

            foreach (var associationSet in model.ConceptualModel.Container.AssociationSets)
            {
                var storeEntitySet = model.StoreModel.Container.EntitySets.SingleOrDefault(x => x.Name.EndsWith("Category"));
                if (storeEntitySet == null) continue;

                var sourceType = (AssociationEndMember)associationSet.ElementType.KeyMembers.First();
                var targetType = (AssociationEndMember)associationSet.ElementType.KeyMembers.ElementAt(1);

                var mapping = new AssociationSetMapping(associationSet, storeEntitySet, model.ConceptualToStoreMapping);

                var s = sourceType.GetEntityType();
                mapping.SourceEndMapping = new EndPropertyMapping(sourceType);
                mapping.SourceEndMapping.AddPropertyMapping(new ScalarPropertyMapping(s.KeyProperties.First(), (EdmProperty)storeEntitySet.ElementType.DeclaredMembers.First(x => x.Name == "FirmId")));

                var t = targetType.GetEntityType();
                mapping.TargetEndMapping = new EndPropertyMapping(targetType);
                mapping.TargetEndMapping.AddPropertyMapping(new ScalarPropertyMapping(t.KeyProperties.First(), storeEntitySet.ElementType.KeyProperties.First()));

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

        private static string ResolveName(IMetadataElementIdentity identity, out string schema)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            schema = null;
            var name = ResolveName(identity);
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

        #endregion

        #region EdmTypeBuilder

        private class EdmTypeBuilder
        {
            private static readonly TypeProvider Provider = new TypeProvider();
            private readonly Dictionary<string, EdmType> _registeredTypes;
            private readonly Dictionary<string, AssociationType> _associationTypes;

            public EdmTypeBuilder(string namespaceName)
            {
                NamespaceName = namespaceName;
                _registeredTypes = new Dictionary<string, EdmType>();
                _associationTypes = new Dictionary<string, AssociationType>();
            }

            public string NamespaceName { get; private set; }

            public IEnumerable<AssociationType> AssociationTypes
            {
                get
                {
                    return _associationTypes.Values;
                }
            }

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
                    _registeredTypes.Add(typeName, complexType = BuildEntityType(typeName, entityElement));
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

                var type = EntityType.Create(typeName, NamespaceName, DataSpace.CSpace, keyNames, properties,
                    //new MetadataProperty[0]
                    new[] { MetadataProperty.CreateAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType", Provider.Resolve(entityElement)) }
                    );

                foreach (var relationElement in entityElement.GetRelations())
                {
                    var propertyName = ResolveName(relationElement.Identity);
                    var structuredType = ResolveComplexType(relationElement.GetTarget());

                    var associationTypeName = typeName + "_" + propertyName;
                    var sourceEnd = AssociationEndMember.Create(
                                                                associationTypeName + "_Source",
                                                                type.GetReferenceType(),
                                                                RelationshipMultiplicity.ZeroOrOne,
                                                                OperationAction.None,
                                                                EmptyMetadata);
                    var targetEnd = AssociationEndMember.Create(
                                                                associationTypeName + "_Target",
                                                                structuredType.GetReferenceType(),
                                                                Convert(relationElement.GetCardinality()),
                                                                OperationAction.None,
                                                                EmptyMetadata);
                    var associationType = AssociationType.Create(
                                                                 associationTypeName,
                                                                 NamespaceName,
                                                                 false,
                                                                 DataSpace.CSpace,
                                                                 sourceEnd,
                                                                 targetEnd,
                                                                 null,
                                                                 EmptyMetadata);

                    _associationTypes.Add(associationType.Name, associationType);

                    var property = NavigationProperty.Create(
                        propertyName, 
                        TypeUsage.Create(structuredType, new Facet[0]),
                        associationType,
                        sourceEnd, targetEnd, EmptyMetadata);

                    type.AddNavigationProperty(property);
                }

                return type;
            }

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

            private static RelationshipMultiplicity Convert(EntityRelationCardinality cardinality)
            {
                switch (cardinality)
                {
                    case EntityRelationCardinality.One:
                        return RelationshipMultiplicity.One;
                    case EntityRelationCardinality.OptionalOne:
                        return RelationshipMultiplicity.ZeroOrOne;
                    case EntityRelationCardinality.Many:
                        return RelationshipMultiplicity.Many;
                    default:
                        throw new ArgumentOutOfRangeException("cardinality");
                }
            }
        }

        #endregion

        #region StoreTypeBuilder

        private class StoreTypeBuilder
        {
            private readonly string _namespaceName;
            private readonly DbProviderManifest _providerManifest;
            private readonly Dictionary<string, EdmType> _registeredTypes;

            public StoreTypeBuilder(string namespaceName, DbProviderManifest providerManifest)
            {
                _namespaceName = namespaceName;
                _providerManifest = providerManifest;
                _registeredTypes = new Dictionary<string, EdmType>();
            }

            public IEnumerable<EdmType> RegisteredTypes
            {
                get
                {
                    return _registeredTypes.Values;
                }
            }

            public EntityType ResolveType(EntityElement entityElement)
            {
                string schema;
                var typeName = ResolveName(entityElement.Identity, out schema);

                EdmType complexType;
                if (!_registeredTypes.TryGetValue(typeName, out complexType))
                {
                    _registeredTypes.Add(typeName, complexType = BuildEntityType(typeName, entityElement));
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
                    var propertyName = propertyElement.Identity.ToName();
                    var propertyType = propertyElement.GetPropertyType();

                    EdmProperty property;
                    if (IsPrimitiveType(propertyType))
                    {
                        var edmProperty = EdmProperty.CreatePrimitive(propertyName, PrimitiveType.GetEdmPrimitiveType(Convert(propertyType)));
                        property = EdmProperty.Create(propertyName, _providerManifest.GetStoreType(edmProperty.TypeUsage));
                    }
                    else if (propertyType == EntityPropertyType.Enum)
                    {
                        throw new NotSupportedException("The enum property is not supported.");
                        //property = EdmProperty.CreateEnum(propertyName, ResolveEnumType(propertyElement));
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


                return EntityType.Create(typeName, _namespaceName, DataSpace.SSpace, keyNames, properties, new MetadataProperty[0]);
            }

            private EnumType BuildEnumType(string typeName, EntityPropertyEnumTypeFeature feature)
            {
                return EnumType.Create(
                    typeName, _namespaceName,
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

    internal static class IdentityExtensions
    {
        public static string ToName(this IMetadataElementIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/').LastOrDefault();
        }
    }
}
