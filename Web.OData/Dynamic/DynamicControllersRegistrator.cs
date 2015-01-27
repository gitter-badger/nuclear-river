using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.Controllers;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData.Dynamic
{
    public sealed class DynamicControllersRegistrator
    {
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

            var dynamicAssembly = CreateDynamicControllersAssembly(boundedContextElement);
            _registry.RegisterDynamicAssembly(dynamicAssembly);
        }

        private static void CopyParentConstructor(TypeBuilder typeBuilder, Type parentType)
        {
            var parentConstructor = parentType.GetConstructors().First();
            var parentParameters = parentConstructor.GetParameters();

            var parameterTypes = parentParameters.Select(x => x.ParameterType).ToArray();
            var constructorBuilder = typeBuilder.DefineConstructor(parentConstructor.Attributes, parentConstructor.CallingConvention, parameterTypes);
            var generator = constructorBuilder.GetILGenerator();

            // load 'this' pointer
            generator.Emit(OpCodes.Ldarg_0);

            for (var i = 1; i <= parentParameters.Length; i++)
            {
                var parentParameter = parentParameters[i - 1];
                constructorBuilder.DefineParameter(i, parentParameter.Attributes, parentParameter.Name);

                generator.Emit(OpCodes.Ldarg_S, i);
            }

            generator.Emit(OpCodes.Call, parentConstructor);
            generator.Emit(OpCodes.Ret);
        }

        private Assembly CreateDynamicControllersAssembly(BoundedContextElement boundedContextElement)
        {
            var assemblyModuleName = boundedContextElement.Identity.Id.Segments.LastOrDefault() + "Controllers";

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyModuleName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyModuleName);


            var entities = boundedContextElement.ConceptualModel.Entities.Where(x => !string.IsNullOrEmpty(x.EntitySetName));

            foreach (var entity in entities)
            {
                var entityType = _typeProvider.Resolve(entity);
                var parentType = typeof(GenericODataController<>).MakeGenericType(entityType);

                var controllerTypeName = entity.EntitySetName + "Controller";
                var typeBuilder = moduleBuilder.DefineType(controllerTypeName, TypeAttributes.Public | TypeAttributes.Sealed, parentType);

                CopyParentConstructor(typeBuilder, parentType);

                typeBuilder.CreateType();
            }

            return assemblyBuilder;
        }
    }
}