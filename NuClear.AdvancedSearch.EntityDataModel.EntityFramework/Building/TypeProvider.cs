using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.EntityDataModel.EntityFramework.Building
{
    // TODO {s.pomadin, 15.01.2015}: introduce and implement interface to resolve type by an entity
    internal sealed class TypeProvider
    {
        private readonly Lazy<AssemblyBuilder> _assemblyBuilder;
        private readonly Lazy<ModuleBuilder> _moduleBuilder;
        private readonly Dictionary<IMetadataElementIdentity, Type> _typesById = new Dictionary<IMetadataElementIdentity, Type>();

        public TypeProvider()
        {
            _assemblyBuilder = new Lazy<AssemblyBuilder>(() => EmitHelper.DefineAssembly("ObjectSpace"));
            _moduleBuilder = new Lazy<ModuleBuilder>(() => _assemblyBuilder.Value.DefineModule("CustomerIntelligence"));
        }

        public Type Resolve(EntityElement entityElement)
        {
            if (entityElement == null)
            {
                throw new ArgumentNullException("entityElement");
            }

            Type type;
            if (!_typesById.TryGetValue(entityElement.Identity, out type))
            {
                _typesById.Add(entityElement.Identity, type = CreateType(entityElement));
            }
            
            return type;
        }

        private Type CreateType(EntityElement entityElement)
        {
            var typeName = ResolveName(entityElement.Identity);
            var tableTypeBuilder = _moduleBuilder.Value.DefineType(typeName);

            foreach (var propertyElement in entityElement.GetProperties())
            {
                var propertyName = ResolveName(propertyElement.Identity);
                var propertyType = ResolveType(propertyElement);

                tableTypeBuilder.DefineProperty(propertyName, propertyType);
            }

            return tableTypeBuilder.CreateType();
        }

        private static string ResolveName(IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/').LastOrDefault();
        }

        private static Type ResolveType(EntityPropertyElement propertyElement)
        {
            var propertyType = propertyElement.GetPropertyType();
            if (propertyType == EntityPropertyType.Enum)
            {
                var feature = propertyElement.Features.OfType<EntityPropertyEnumTypeFeature>().SingleOrDefault();
                if (feature != null)
                {
                    propertyType = feature.UnderlyingType;
                }
            }
            return ConvertType(propertyType);
        }

        private static Type ConvertType(EntityPropertyType propertyType)
        {
            switch (propertyType)
            {
                case EntityPropertyType.Byte:
                    return typeof(byte);
                case EntityPropertyType.Int16:
                case EntityPropertyType.Int32:
                    return typeof(int);
                case EntityPropertyType.Int64:
                    return typeof(long);
                case EntityPropertyType.Single:
                    return typeof(float);
                case EntityPropertyType.Double:
                    return typeof(double);
                case EntityPropertyType.Decimal:
                    return typeof(decimal);
                case EntityPropertyType.Boolean:
                    return typeof(bool);
                case EntityPropertyType.String:
                    return typeof(string);
                case EntityPropertyType.DateTime:
                    return typeof(DateTime);
                default:
                    throw new ArgumentOutOfRangeException("propertyType");
            }
        }
    }
}