using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Provider;

namespace NuClear.EntityDataModel.EntityFramework.Building
{
    public sealed class EdmxModelBuilder
    {
        private readonly DbProviderInfo _providerInfo;
        private readonly ITypeProvider _typeProvider;
        private readonly Dictionary<Type, EntityElement> _elementsByType = new Dictionary<Type, EntityElement>();

        public EdmxModelBuilder(DbProviderInfo providerInfo, ITypeProvider typeProvider = null)
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

            var builder = CreateModelBuilder();

            if (context.ConceptualModel != null)
            {
                foreach (var entityElement in context.ConceptualModel.Entities)
                {
                    var entityType = _typeProvider.Resolve(entityElement);
                    var configuration = RegisterType(builder, entityType);

                    _elementsByType.Add(entityType, entityElement);

                    var entitySetName = entityElement.GetEntitySetName() ?? entityElement.ResolveName();
                    var keyNames = entityElement.GetKeyProperties().Select(p => p.ResolveName()).ToArray();
                    configuration.Configure(x => x
                        .HasEntitySetName(entitySetName)
                        .HasKey(keyNames));

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
            }

            var model = builder.Build(_providerInfo);

            return model;
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
            AddCustomConventions(builder);
            
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

        private void AddCustomConventions(DbModelBuilder builder)
        {
            //builder.Conventions.Add(new EntityKeyConvention());
            //builder.Conventions.Add(new EntitySetNameConvention());
            //builder.Conventions.Add(new TableMappingConvention());
        }

        private class EntityKeyConvention : IConceptualModelConvention<EntityType>
        {
            private readonly IReadOnlyDictionary<Type, EntityElement> _elementsByType;

            public EntityKeyConvention(IReadOnlyDictionary<Type, EntityElement> elementsByType)
            {
                _elementsByType = elementsByType;
            }

            public void Apply(EntityType item, DbModel model)
            {
            }
        }

        private class EntitySetNameConvention : IConceptualModelConvention<EntityType>
        {
            public EntitySetNameConvention()
            {
            }

            public void Apply(EntityType item, DbModel model)
            {
                var meta = item.MetadataProperties.Where(x => x.IsAnnotation);
            }
        }

        private class TableMappingConvention : Convention//, IConceptualModelConvention<EntityType>
        {
            public TableMappingConvention()
            {
                //Types().Having(t => t.HasElementType)
            }

            public void Apply(EntityType item, DbModel model)
            {
                //item.
            }
        }
    }
}
