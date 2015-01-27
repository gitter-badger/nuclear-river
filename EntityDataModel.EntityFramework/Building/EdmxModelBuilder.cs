using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Emit;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building
{
    public sealed class EdmxModelBuilder
    {
        private readonly DbProviderInfo _providerInfo;
        private readonly IMetadataProvider _metadataProvider;
        private readonly ITypeProvider _typeProvider;

        public EdmxModelBuilder(DbProviderInfo providerInfo, IMetadataProvider metadataProvider)
            : this(providerInfo, metadataProvider, new EmitTypeProvider())
        {
        }

        public EdmxModelBuilder(DbProviderInfo providerInfo, IMetadataProvider metadataProvider, ITypeProvider typeProvider)
        {
            if (providerInfo == null)
            {
                throw new ArgumentNullException("providerInfo");
            }
            if (metadataProvider == null)
            {
                throw new ArgumentNullException("metadataProvider");
            }
            if (typeProvider == null)
            {
                throw new ArgumentNullException("typeProvider");
            }

            _providerInfo = providerInfo;
            _metadataProvider = metadataProvider;
            _typeProvider = typeProvider;
        }

        public DbModel Build(Uri contextUrl)
        {
            if (contextUrl == null)
            {
                throw new ArgumentNullException("contextUrl");
            }

            BoundedContextElement boundedContextElement;
            _metadataProvider.TryGetMetadata(contextUrl, out boundedContextElement);
            if (boundedContextElement == null || boundedContextElement.ConceptualModel == null)
            {
                return null;
            }

            return Build(new BuildContext(_metadataProvider, _typeProvider, boundedContextElement));
        }

        private DbModel Build(BuildContext context)
        {
            var builder = CreateModelBuilder(context);

            foreach (var entityElement in context.EntityTypes)
            {
                var entityType = context.ResolveType(entityElement);
                var configuration = RegisterType(builder, entityType);

                ConfigureEntityType(configuration, entityElement, context);
            }

            return builder.Build(_providerInfo);
        }

        private static void ConfigureEntityType(TypeConventionConfiguration configuration, EntityElement entityElement, BuildContext context)
        {
            // add annotation
            configuration.Configure(x => x.HasTableAnnotation("EntityId", entityElement.Identity.Id));

            // update entity set name
            var entitySetName = entityElement.EntitySetName ?? entityElement.ResolveName();
            configuration.Configure(x => x.HasEntitySetName(entitySetName));

            // declare keys
            var keyNames = entityElement.KeyProperties.Select(p => p.ResolveName());
            configuration.Configure(x => x.HasKey(keyNames));

            // specify table schema and name
            var storeEntityElement = context.LookupMappedEntity(entityElement);
            if (storeEntityElement != null)
            {
                string schemaName;
                var tableName = storeEntityElement.ResolveName();
                ParseTableName(ref tableName, out schemaName);

                configuration.Configure(x => x.ToTable(tableName, schemaName));
                configuration.Configure(x => x.HasTableAnnotation("EntityId", storeEntityElement.Identity.Id));
            }

            foreach (var propertyElement in entityElement.Properties)
            {
                var propertyType = propertyElement.PropertyType;
                if (propertyType == EntityPropertyType.Enum) continue;

                var propertyName = propertyElement.ResolveName();
                if (propertyElement.IsNullable)
                {
                    configuration.Configure(x => x.Property(propertyName).IsOptional());
                }
                else
                {
                    configuration.Configure(x => x.Property(propertyName).IsRequired());
                }
            }
        }

        private static void ParseTableName(ref string tableName, out string schemaName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException("tableName");
            }

            schemaName = null;
            
            var index = tableName.IndexOf('.');
            if (index >= 0)
            {
                schemaName = tableName.Substring(0, index);
                tableName = tableName.Substring(index + 1);
            }
        }

        private static TypeConventionConfiguration RegisterType(DbModelBuilder builder, Type entityType)
        {
            builder.RegisterEntityType(entityType);

            return builder.Types().Where(x => x == entityType);
        }

        private DbModelBuilder CreateModelBuilder(BuildContext context)
        {
            var builder = new DbModelBuilder();
            
            DropDefaultConventions(builder);
            builder.Conventions.Add(new ForeignKeyMappingConvention(context));
            
            return builder;
        }

        private static void DropDefaultConventions(DbModelBuilder builder)
        {
            // conceptual model conventions
            builder.Conventions.Remove<PluralizingEntitySetNameConvention>();

            // store model conventions
            builder.Conventions.Remove<ForeignKeyIndexConvention>();
            builder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        private class ForeignKeyMappingConvention : IStoreModelConvention<AssociationType>
        {
            private const string AnnotationKey = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:EntityId";
            private readonly BuildContext _context;

            public ForeignKeyMappingConvention(BuildContext context)
            {
                _context = context;
            }

            public void Apply(AssociationType item, DbModel model)
            {
                if (item.IsForeignKey && item.Constraint != null)
                {
                    var sourceId = ResolveId(item.Constraint.FromRole.GetEntityType());
                    var targetId = ResolveId(item.Constraint.ToRole.GetEntityType());

                    if (sourceId != null & targetId != null)
                    {
                        var sourceElement = _context.LookupEntity(sourceId);
                        var targetElement = _context.LookupEntity(targetId);

                        if (sourceElement != null & targetElement != null)
                        {
                            var relation = targetElement.Relations.FirstOrDefault(x => x.Target.ResolveName() == sourceElement.ResolveName());
                            if (relation != null)
                            {
                                item.Constraint.ToProperties.First().Name = relation.ResolveName();
                            }
                        }
                    }
                }
            }

            private static Uri ResolveId(EntityType entityType)
            {
                MetadataProperty property;
                if (entityType.MetadataProperties.TryGetValue(AnnotationKey, false, out property))
                {
                    return (Uri)property.Value;
                }
                return null;
            }
        }

        private class BuildContext
        {
            private readonly IMetadataProvider _metadataProvider;
            private readonly BoundedContextElement _boundedContextElement;
            private readonly ITypeProvider _typeProvider;
            private readonly Dictionary<Uri, IMetadataElementIdentity> _storeEntities;

            public BuildContext(IMetadataProvider metadataProvider, ITypeProvider typeProvider, BoundedContextElement boundedContextElement)
            {
                _metadataProvider = metadataProvider;
                _boundedContextElement = boundedContextElement;
                _typeProvider = typeProvider;

                var storeModelId = boundedContextElement.StoreModel != null ? new Uri(boundedContextElement.StoreModel.Identity.Id + "/") : null;
                _storeEntities = boundedContextElement.StoreModel != null
                    ? boundedContextElement.StoreModel.Entities
                    .ToDictionary(x => storeModelId.MakeRelativeUri(x.Identity.Id), x => x.Identity)
                    : new Dictionary<Uri, IMetadataElementIdentity>();
            }

            public IEnumerable<EntityElement> EntityTypes
            {
                get
                {
                    return _boundedContextElement.ConceptualModel.Entities;
                }
            }

            public EntityElement LookupEntity(Uri entityUrl)
            {
                EntityElement entityElement;
                return _metadataProvider.TryGetMetadata(entityUrl, out entityElement) ? entityElement : null;
            }

            public EntityElement LookupMappedEntity(EntityElement entityElement)
            {
                var mappedEntity = entityElement.MappedEntity;
                if (mappedEntity == null)
                {
                    return null;
                }

                return (EntityElement)mappedEntity;

                //                IMetadataElementIdentity storeElementIdentity;
                //                return _storeEntities.TryGetValue(mappedEntity.Id, out storeElementIdentity) ? LookupEntity(storeElementIdentity.Id) : null;
            }

            public Type ResolveType(EntityElement entityElement)
            {
                return _typeProvider.Resolve(entityElement);
            }
        }
    }
}
