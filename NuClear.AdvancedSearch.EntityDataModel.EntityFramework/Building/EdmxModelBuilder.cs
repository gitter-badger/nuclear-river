using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;

namespace NuClear.EntityDataModel.EntityFramework.Building
{
    public sealed class EdmxModelBuilder
    {
        private readonly DbProviderInfo _providerInfo;
        private readonly ITypeProvider _typeProvider;

        public EdmxModelBuilder(DbProviderInfo providerInfo, ITypeProvider typeProvider = null)
        {
            if (providerInfo == null)
            {
                throw new ArgumentNullException("providerInfo");
            }

            _providerInfo = providerInfo;
            _typeProvider = typeProvider ?? new EmitTypeProvider();
        }

        public DbModel Build(IMetadataProvider metadataProvider, Uri contextUrl)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException("metadataProvider");
            }
            if (contextUrl == null)
            {
                throw new ArgumentNullException("contextUrl");
            }

            BoundedContextElement boundedContextElement;
            metadataProvider.TryGetMetadata(contextUrl, out boundedContextElement);
            if (boundedContextElement == null || boundedContextElement.ConceptualModel == null)
            {
                return null;
            }

            return Build(new BuildContext(metadataProvider, boundedContextElement, _typeProvider));
        }

        private DbModel Build(BuildContext context)
        {
            var builder = CreateModelBuilder();

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
            // update entity set name
            var entitySetName = entityElement.GetEntitySetName() ?? entityElement.ResolveName();
            configuration.Configure(x => x.HasEntitySetName(entitySetName));

            // declare keys
            var keyNames = entityElement.GetKeyProperties().Select(p => p.ResolveName());
            configuration.Configure(x => x.HasKey(keyNames));

            // specify table schema and name
            var storeEntityElement = context.LookupMappedEntity(entityElement);
            if (storeEntityElement != null)
            {
                string schemaName;
                var tableName = storeEntityElement.ResolveName(out schemaName);
                configuration.Configure(x => x.ToTable(tableName, schemaName));
            }

            foreach (var propertyElement in entityElement.GetProperties())
            {
                var propertyType = propertyElement.GetPropertyType();
                if (propertyType == EntityPropertyType.Enum) continue;

                var propertyName = propertyElement.ResolveName();
                if (propertyElement.IsNullable())
                {
                    configuration.Configure(x => x.Property(propertyName).IsOptional());
                }
                else
                {
                    configuration.Configure(x => x.Property(propertyName).IsRequired());
                }
            }
        }

        private static TypeConventionConfiguration RegisterType(DbModelBuilder builder, Type entityType)
        {
            builder.RegisterEntityType(entityType);

            return builder.Types().Where(x => x == entityType);
        }

        private DbModelBuilder CreateModelBuilder()
        {
            var builder = new DbModelBuilder();
            
            DropDefaultConventions(builder);
            
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

        private class BuildContext
        {
            private readonly IMetadataProvider _metadataProvider;
            private readonly BoundedContextElement _boundedContextElement;
            private readonly ITypeProvider _typeProvider;
            private readonly Dictionary<Uri, IMetadataElementIdentity> _storeEntities;

            public BuildContext(IMetadataProvider metadataProvider, BoundedContextElement boundedContextElement, ITypeProvider typeProvider)
            {
                _metadataProvider = metadataProvider;
                _boundedContextElement = boundedContextElement;
                _typeProvider = typeProvider;

                var storeModelId = boundedContextElement.StoreModel != null ? new Uri(boundedContextElement.StoreModel.Identity.Id + "/") : null;
                _storeEntities = boundedContextElement.StoreModel != null
                    ? boundedContextElement.StoreModel.GetFlattenEntities()
                    .ToDictionary(x => storeModelId.MakeRelativeUri(x.Identity.Id), x => x.Identity)
                    : new Dictionary<Uri, IMetadataElementIdentity>();
            }

            public IEnumerable<EntityElement> Entities
            {
                get
                {
                    return _boundedContextElement.ConceptualModel.Entities;
                }
            }

            public IEnumerable<EntityElement> EntityTypes
            {
                get
                {
                    return _boundedContextElement.ConceptualModel.GetFlattenEntities();
                }
            }

            public EntityElement LookupEntity(Uri entityUrl)
            {
                EntityElement entityElement;
                return _metadataProvider.TryGetMetadata(entityUrl, out entityElement) ? entityElement : null;
            }

            public EntityElement LookupMappedEntity(EntityElement entityElement)
            {
                var mappedId = entityElement.GetMappedEntityIdentity();
                if (mappedId == null)
                {
                    return null;
                }

                IMetadataElementIdentity storeElementIdentity;
                return _storeEntities.TryGetValue(mappedId.Id, out storeElementIdentity) ? LookupEntity(storeElementIdentity.Id) : null;
            }

            public Type ResolveType(EntityElement entityElement)
            {
                return _typeProvider.Resolve(entityElement);
            }
        }
    }
}
