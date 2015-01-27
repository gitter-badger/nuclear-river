using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Emit
{
    public sealed class EmitTypeProvider : ITypeProvider
    {
        private const string CustomCodeName = "CustomCode";

        private readonly Lazy<AssemblyBuilder> _assemblyBuilder;
        private readonly Lazy<ModuleBuilder> _moduleBuilder;
        private readonly Dictionary<IMetadataElementIdentity, Type> _typesById = new Dictionary<IMetadataElementIdentity, Type>();

        public EmitTypeProvider()
        {
            _assemblyBuilder = new Lazy<AssemblyBuilder>(() => EmitHelper.DefineAssembly(CustomCodeName));
            _moduleBuilder = new Lazy<ModuleBuilder>(() => AssemblyBuilder.DefineModule(CustomCodeName));
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

        private AssemblyBuilder AssemblyBuilder
        {
            get
            {
                return _assemblyBuilder.Value;
            }
        }

        private ModuleBuilder ModuleBuilder
        {
            get
            {
                return _moduleBuilder.Value;
            }
        }

        private Type CreateType(EntityElement entityElement)
        {
            var typeName = entityElement.ResolveFullName();
            var tableTypeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public);

            foreach (var propertyElement in entityElement.Properties)
            {
                var propertyName = propertyElement.ResolveName();
                var propertyType = ResolveType(propertyElement);

                tableTypeBuilder.DefineProperty(propertyName, propertyType);
            }

            foreach (var relationElement in entityElement.Relations)
            {
                var relationTarget = relationElement.Target;
                var relationCardinality = relationElement.Cardinality;

                var propertyName = relationElement.ResolveName();
                var propertyType = CreateRelationType(Resolve(relationTarget), relationCardinality);

                tableTypeBuilder.DefineProperty(propertyName, propertyType);
            }

            return tableTypeBuilder.CreateType();
        }

        private Type ResolveType(EntityPropertyElement propertyElement)
        {
            var propertyType = propertyElement.PropertyType;
            if (propertyType == EntityPropertyType.Enum)
            {
                return CreateEnum(propertyElement);
            }
            return ConvertType(propertyType);
        }

        private Type CreateEnum(EntityPropertyElement propertyElement)
        {
            var typeName = propertyElement.EnumName;
            var underlyingType = ConvertType(propertyElement.EnumUnderlyingType);

            var typeBuilder = ModuleBuilder.DefineEnum(typeName, underlyingType);

            foreach (var member in propertyElement.EnumMembers)
            {
                typeBuilder.DefineLiteral(member.Key, Convert.ChangeType(member.Value, underlyingType));
            }

            return typeBuilder.CreateType();
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
                case EntityPropertyType.DateTimeOffset:
                    return typeof(DateTimeOffset);
                default:
                    throw new ArgumentOutOfRangeException("propertyType");
            }
        }

        private static Type CreateRelationType(Type entityType, EntityRelationCardinality cardinality)
        {
            switch (cardinality)
            {
                case EntityRelationCardinality.One:
                case EntityRelationCardinality.OptionalOne:
                    return entityType;
                case EntityRelationCardinality.Many:
                    return typeof(ICollection<>).MakeGenericType(entityType);
                default:
                    throw new ArgumentOutOfRangeException("cardinality");
            }
        }
    }
}