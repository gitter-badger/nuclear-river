using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.Controllers;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData
{
    public sealed class DynamicControllersRegistrator
    {
        private const string AssemblyModuleName = "ODataControllers";

        private readonly IMetadataProvider _metadataProvider;
        private readonly ITypeProvider _typeProvider;
        private readonly IDynamicAssembliesRegistry _registry;

        public DynamicControllersRegistrator(IMetadataProvider metadataProvider, ITypeProvider typeProvider, IDynamicAssembliesRegistry registry)
        {
            _metadataProvider = metadataProvider;
            _typeProvider = typeProvider;
            _registry = registry;
        }

        public void RegisterDynamicControllers(Uri uri)
        {
            BoundedContextElement boundedContextElement;
            if (!_metadataProvider.TryGetMetadata(uri, out boundedContextElement))
            {
                throw new ArgumentException();
            }

            var dynamicAssembly = CreateDynamicControllersAssembly(boundedContextElement.ConceptualModel.Entities);
            _registry.RegisterDynamicAssembly(dynamicAssembly);
        }

        private Assembly CreateDynamicControllersAssembly(IEnumerable<EntityElement> entities)
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(AssemblyModuleName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(AssemblyModuleName);

            foreach (var entity in entities)
            {
                var entityType = _typeProvider.Resolve(entity);
                var parentType = typeof(GenericODataController<>).MakeGenericType(entityType);

                var controllerTypeName = entity.GetEntitySetName() + "Controller";
                var typeBuilder = moduleBuilder.DefineType(controllerTypeName, TypeAttributes.Public | TypeAttributes.Sealed, parentType);

                typeBuilder.CreateType();
            }

            return assemblyBuilder;
        }
    }
}