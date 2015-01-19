using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
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
    }
}
