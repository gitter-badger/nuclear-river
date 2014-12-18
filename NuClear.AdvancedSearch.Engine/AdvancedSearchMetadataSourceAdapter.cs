using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.Engine
{
    public sealed class AdvancedSearchMetadataSourceAdapter : IEdmModelSource
    {
        private readonly string _namespaceName;
        private readonly IReadOnlyCollection<EdmEntityType> _entities;
        private readonly IReadOnlyCollection<EdmEntityRelationInfo> _relations;

        public AdvancedSearchMetadataSourceAdapter(BoundedContextElement boundedContext)
        {
            if (boundedContext == null)
            {
                throw new ArgumentNullException("boundedContext");
            }

            var entityElements = new Dictionary<IMetadataElementIdentity, EdmEntityType>();
            var relationElements = new List<Tuple<IMetadataElementIdentity, EntityRelationElement>>();

            foreach (var entity in boundedContext.RootEntities)
            {
                entityElements.Add(entity.Identity, Convert(entity));
//                Traverse(rootEntity, 
//                         delegate(EntityElement x)
//                         {
//                             if (!entityElements.ContainsKey(x.Identity))
//                             {
//                                 entityElements.Add(x.Identity, Convert(x));
//                             }
//
//                             //relationElements.AddRange(x.Relations.Select(relationElement => Tuple.Create(x.Identity, relationElement)));
//                         });
            }

            _namespaceName = ResolveNamespace(boundedContext.Identity);
            _entities = new List<EdmEntityType>();
            _relations = new List<EdmEntityRelationInfo>();

            _entities = entityElements.Values.ToList();
//            _relations = relationElements.Select(x => Convert(entityElements, x.Item1, x.Item2)).ToList();
        }

        public string Namespace
        {
            get
            {
                return _namespaceName;
            }
        }

        public IReadOnlyCollection<EdmEntityType> Entities
        {
            get
            {
                return _entities;
            }
        }

        public IReadOnlyCollection<EdmEntityRelationInfo> Relations
        {
            get
            {
                return _relations;
            }
        }

        private static EdmEntityType Convert(EntityElement entityElement)
        {
            var name = ResolveName(entityElement.Identity);
            var properties = entityElement.Properties.ToDictionary(x => x.Identity, Convert);
            //var keys = entityElement.Keys.Select(x => properties[x.Identity]).ToArray();

            var entityInfo = new EdmEntityType(name, properties.Values);
            return entityInfo;
        }

        private static EdmEntityPropertyInfo Convert(EntityPropertyElement propertyElement)
        {
            var name = ResolveName(propertyElement.Identity);
            return new EdmEntityPropertyInfo(name, CreateTypeReference(propertyElement));
        }

        private static EdmTypeReference CreateTypeReference(EntityPropertyElement propertyElement)
        {
            return new EdmTypeReference(CreateType(propertyElement));
        }

        private static EdmType CreateType(EntityPropertyElement propertyElement)
        {
            if (propertyElement.PropertyType == EntityPropertyType.Enum)
            {
                var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().Single();
                return new EdmEnumType(Convert(feature.UnderlyingType), feature.Members);
            }
            
            return new EdmPrimitiveType(Convert(propertyElement.PropertyType));
        }

//        private static EdmEntityRelationInfo Convert(
//            IReadOnlyDictionary<IMetadataElementIdentity,EdmEntityInfo> entityInfos,
//            IMetadataElementIdentity sourceEntityId,
//            EntityRelationElement relationElement)
//        {
//            var name = ResolveName(relationElement.Identity);
//            var sourceEntity = entityInfos[sourceEntityId];
//            var targetEntity = entityInfos[relationElement.Target.Identity];
//            var multiplicity = Convert(relationElement.Cardinality);
//
//            return new EdmEntityRelationInfo(name, sourceEntity, targetEntity, multiplicity);
//        }
//
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

        private static EdmEntityRelationMultiplicity Convert(EntityRelationCardinality cardinality)
        {
            switch (cardinality)
            {
                case EntityRelationCardinality.One:
                    return EdmEntityRelationMultiplicity.One;
                case EntityRelationCardinality.OptionalOne:
                    return EdmEntityRelationMultiplicity.ZeroOrOne;
                case EntityRelationCardinality.Many:
                    return EdmEntityRelationMultiplicity.Many;
                default:
                    throw new ArgumentOutOfRangeException("cardinality");
            }
        }

        private static string ResolveNamespace(IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Replace("/", ".");
        }

        private static string ResolveName(IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.ToString().Split('/').LastOrDefault();
        }

        private static void Traverse(EntityElement entityElement, Action<EntityElement> action)
        {
            action(entityElement);
            foreach (var relation in entityElement.Relations)
            {
                Traverse(relation.Target, action);
            }
        }
    }
}