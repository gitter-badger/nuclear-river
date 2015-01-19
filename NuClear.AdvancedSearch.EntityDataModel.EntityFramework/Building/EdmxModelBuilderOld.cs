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
    public sealed class EdmxModelBuilderOld
    {
        private static readonly IEnumerable<MetadataProperty> EmptyMetadata = new MetadataProperty[0];

        private readonly DbProviderInfo _providerInfo;
        private readonly ITypeProvider _typeProvider;

        public EdmxModelBuilderOld(DbProviderInfo providerInfo, ITypeProvider typeProvider = null)
        {
            if (providerInfo == null)
            {
                throw new ArgumentNullException("providerInfo");
            }

            _providerInfo = providerInfo;
            _typeProvider = typeProvider ?? new EmitTypeProvider();
        }

        public DbModel Build(IMetadataProvider metadataProvider, Uri uri)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException("metadataProvider");
            }

            BoundedContextElement context;
            metadataProvider.TryGetMetadata(uri, out context);

            return Build(context);
        }

        public DbModel Build(BoundedContextElement context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var builder = new DbModelBuilder();

            if (context.ConceptualModel != null)
            {
                foreach (var entityElement in context.ConceptualModel.Entities)
                {
                    var entityType = _typeProvider.Resolve(entityElement);

                    builder.RegisterEntityType(entityType);

                    var configuration = builder.Types().Where(x => x == entityType);

                    var entitySetName = entityElement.GetEntitySetName() ?? ResolveName(entityElement.Identity);
                    var keyNames = entityElement.GetKeyProperties().Select(p => ResolveName(p.Identity)).ToArray();
                    configuration.Configure(x => x
                        .HasEntitySetName(entitySetName)
                        .HasKey(keyNames));
                }
            }

            var model = builder.Build(_providerInfo);

            return model;
        }

        public DbModel BuildStraightly(BoundedContextElement context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var builder = new DbModelBuilder();
            var model = builder.Build(_providerInfo);

            var cmNamespaceName = ResolveNamespaceName(context.Identity);
            var smNamespaceName = ResolveNamespaceName(context.Identity) + ".Store"; // FIXME {s.pomadin, 13.01.2015}: has to be improved

            var conceptualContext = BuildEdmModel(model.ConceptualModel, context.ConceptualModel, cmNamespaceName);
            var storeContext = BuildStoreModel(model.StoreModel, context.StoreModel, smNamespaceName, model.ProviderManifest);
            //BuildOneToOneMapping(model);
            BuildMapping(model, context.ConceptualToStoreMapping, conceptualContext, storeContext);

            return model;
        }

        private ModelContext BuildEdmModel(EdmModel model, StructuralModelElement conceptualModel, string namespaceName)
        {
            if (conceptualModel == null)
            {
                return null;
            }

            var typeBuilder = new EdmTypeBuilder(namespaceName, _typeProvider);
            var entitySets = new Dictionary<IMetadataElementIdentity, EntitySet>();

            // process entities
            foreach (var entityElement in conceptualModel.Entities)
            {
                typeBuilder.ResolveComplexType(entityElement);
            }

//            foreach (var entityElement in conceptualModel.Entities)
//            {
//                var entitySetName = entityElement.GetEntitySetName() ?? ResolveName(entityElement.Identity);
//                var entityType = typeBuilder.ResolveComplexType(entityElement);
//
//                //model.Container.AddEntitySetBase(EntitySet.Create(entitySetName, typeBuilder.NamespaceName, entityType.Name, null, entityType, new MetadataProperty[0]));
//                //model.Container.AddEntitySetBase(EntitySet.Create(entitySetName, null, null, null, entityType, new MetadataProperty[0]));
//            }

            foreach (var pair in typeBuilder.EntityTypes)
            {
                var entityElement = pair.Key;
                var entityType = pair.Value;

//                string schemaName;
//                var tableName = ResolveName(entityElement.Identity, out schemaName);
//                var entityName = entityElement.GetEntitySetName() ?? tableName;

                var entitySet = EntitySet.Create(entityType.Name, null, null, null, entityType, EmptyMetadata);

                model.Container.AddEntitySetBase(entitySet);
                model.AddItem(entityType);

                entitySets.Add(entityElement.Identity, entitySet);
            }

            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
//                var entityType = registeredType as EntityType;
//                if (entityType != null)
//                {
//                    var entitySet = EntitySet.Create(entityType.Name, null, null, null, entityType, EmptyMetadata);
//
//                    model.Container.AddEntitySetBase(entitySet);
//                    model.AddItem(entityType);
//                }
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

            return new ModelContext(entitySets);
        }

        private static ModelContext BuildStoreModel(EdmModel model, StructuralModelElement storeModel, string namespaceName, DbProviderManifest providerManifest)
        {
            if (storeModel == null)
            {
                return null;
            }

            var typeBuilder = new StoreTypeBuilder(namespaceName, providerManifest);
            var entitySets = new Dictionary<EntityType, EntitySet>();
            var entitySets2 = new Dictionary<IMetadataElementIdentity, EntitySet>();

            // process entities
            foreach (var entityElement in storeModel.Entities)
            {
                typeBuilder.ResolveType(entityElement);
            }

            // register entity types and sets
            foreach (var pair in typeBuilder.EntityTypes)
            {
                var entityElement = pair.Key;
                var entityType = pair.Value;

                string schemaName;
                var tableName = ResolveName(entityElement.Identity, out schemaName);
                var entityName = entityElement.GetEntitySetName() ?? tableName;
                var entitySet = EntitySet.Create(entityName, schemaName, tableName, null, entityType, EmptyMetadata);

                model.AddItem(entityType);
                model.Container.AddEntitySetBase(entitySet);
                
                entitySets.Add(entityType, entitySet);
                entitySets2.Add(entityElement.Identity, entitySet);
            }

            // register association types and sets
            foreach (var associationType in typeBuilder.AssociationTypes)
            {
                var sourceEndMember = (AssociationEndMember)associationType.KeyMembers.First();
                var targetEndMember = (AssociationEndMember)associationType.KeyMembers.Last();

                var sourceType = sourceEndMember.GetEntityType();
                var targetType = targetEndMember.GetEntityType();

                var sourceEntitySet = entitySets[sourceType];
                var targetEntitySet = entitySets[targetType];
                
                var associationSet = AssociationSet.Create(associationType.Name, associationType, sourceEntitySet, targetEntitySet, EmptyMetadata);

                model.AddItem(associationType);
                model.Container.AddEntitySetBase(associationSet);
            }

            return new ModelContext(entitySets2);
        }

        private void BuildMapping(DbModel model, ModelMappingElement mappingElement, ModelContext conceptualContext, ModelContext storeContext)
        {
            foreach (var entityMapping in mappingElement.Mappings())
            {
                var conceptualEntitySet = conceptualContext.EntitySets[entityMapping.ConceptualEntityIdentity];
                if (conceptualEntitySet == null) continue;

                var mapping = new EntitySetMapping(conceptualEntitySet, model.ConceptualToStoreMapping);

                model.ConceptualToStoreMapping.AddSetMapping(mapping);
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

//            foreach (var associationSet in model.ConceptualModel.Container.AssociationSets)
//            {
//                var storeEntitySet = model.StoreModel.Container.EntitySets.SingleOrDefault(x => x.Name.EndsWith("Category"));
//                if (storeEntitySet == null) continue;
//
//                var sourceType = (AssociationEndMember)associationSet.ElementType.KeyMembers.First();
//                var targetType = (AssociationEndMember)associationSet.ElementType.KeyMembers.ElementAt(1);
//
//                var mapping = new AssociationSetMapping(associationSet, storeEntitySet, model.ConceptualToStoreMapping);
//
//                var s = sourceType.GetEntityType();
//                mapping.SourceEndMapping = new EndPropertyMapping(sourceType);
//                mapping.SourceEndMapping.AddPropertyMapping(new ScalarPropertyMapping(s.KeyProperties.First(), (EdmProperty)storeEntitySet.ElementType.DeclaredMembers.First(x => x.Name == "FirmId")));
//
//                var t = targetType.GetEntityType();
//                mapping.TargetEndMapping = new EndPropertyMapping(targetType);
//                mapping.TargetEndMapping.AddPropertyMapping(new ScalarPropertyMapping(t.KeyProperties.First(), storeEntitySet.ElementType.KeyProperties.First()));
//
//                model.ConceptualToStoreMapping.AddSetMapping(mapping);
//            }
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
            private readonly ITypeProvider _typeProvider;
            private readonly Dictionary<string, EdmType> _registeredTypes;
            private readonly Dictionary<string, AssociationType> _associationTypes;
            private readonly string _namespaceName;
            private readonly Dictionary<EntityElement, EntityType> _entityTypes = new Dictionary<EntityElement, EntityType>();

            public EdmTypeBuilder(string namespaceName, ITypeProvider typeProvider)
            {
                _namespaceName = namespaceName;
                _typeProvider = typeProvider;
                _registeredTypes = new Dictionary<string, EdmType>();
                _associationTypes = new Dictionary<string, AssociationType>();
            }

            public IEnumerable<AssociationType> AssociationTypes
            {
                get
                {
                    return _associationTypes.Values;
                }
            }

            public IReadOnlyDictionary<EntityElement, EntityType> EntityTypes
            {
                get
                {
                    return _entityTypes;
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
                    var entityType = BuildEntityType(typeName, entityElement);
                    _entityTypes.Add(entityElement, entityType);
                    _registeredTypes.Add(typeName, complexType = entityType);
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

                var type = EntityType.Create(typeName, _namespaceName, DataSpace.CSpace, keyNames, properties,
                    //new MetadataProperty[0]
                    new[] { MetadataProperty.CreateAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType", _typeProvider.Resolve(entityElement)) }
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
                                                                 _namespaceName,
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
            private readonly Dictionary<EntityElement, EntityType> _entityTypes = new Dictionary<EntityElement, EntityType>();
            private readonly List<AssociationType> _associationTypes = new List<AssociationType>();

            private readonly string _namespaceName;
            private readonly DbProviderManifest _providerManifest;

            public StoreTypeBuilder(string namespaceName, DbProviderManifest providerManifest)
            {
                _namespaceName = namespaceName;
                _providerManifest = providerManifest;
            }

            public IReadOnlyDictionary<EntityElement, EntityType> EntityTypes
            {
                get
                {
                    return _entityTypes;
                }
            }

            public IEnumerable<AssociationType> AssociationTypes
            {
                get
                {
                    return _associationTypes;
                }
            }

            public EntityType ResolveType(EntityElement entityElement)
            {
                string schema;
                var typeName = ResolveName(entityElement.Identity, out schema);

                EntityType complexType;
                if (!_entityTypes.TryGetValue(entityElement, out complexType))
                {
                    _entityTypes.Add(entityElement, complexType = BuildEntityType(typeName, entityElement));
                }

                return complexType;
            }

            private EntityType BuildEntityType(string typeName, EntityElement entityElement)
            {
                var keyIds = new HashSet<IMetadataElementIdentity>(entityElement.GetKeyProperties().Select(x => x.Identity));

                var keyNames = new List<string>();
                var properties = new List<EdmProperty>();

                foreach (var propertyElement in entityElement.GetProperties())
                {
                    var propertyName = propertyElement.ResolveName();
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

                var entityType = EntityType.Create(typeName, _namespaceName, DataSpace.SSpace, keyNames, properties, EmptyMetadata);

                foreach (var relationElement in entityElement.GetRelations())
                {
                    var relationName = ResolveName(relationElement.Identity);
                    var targetType = ResolveType(relationElement.GetTarget());

                    var associationTypeName = typeName + "_" + relationName;
                    var sourceMember = AssociationEndMember.Create(
                        associationTypeName + "_Source",
                        entityType.GetReferenceType(),
                        RelationshipMultiplicity.ZeroOrOne,
                        OperationAction.None,
                        EmptyMetadata);
                    var targetMember = AssociationEndMember.Create(
                        associationTypeName + "_Target",
                        targetType.GetReferenceType(),
                        Convert(relationElement.GetCardinality()),
                        OperationAction.None,
                        EmptyMetadata);

                    var associationType = AssociationType.Create(associationTypeName, _namespaceName, false, DataSpace.SSpace, sourceMember, targetMember, null, EmptyMetadata);

                    _associationTypes.Add(associationType);
                }

                return entityType;
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

        private class ModelContext
        {
            public ModelContext(Dictionary<IMetadataElementIdentity, EntitySet> entitySets)
            {
                EntitySets = entitySets;
            }

            public Dictionary<IMetadataElementIdentity, EntitySet> EntitySets { get; private set; }
        }
    }
}
