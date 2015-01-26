using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class GenericODataController<T> : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<T> options)
        {
            var content = Enumerable.Empty<T>();

            return Ok(content);
        }
    }

    public sealed class ControllerAssemblyBuilder
    {
        private const string AssemblyModuleName = "ODataControllers";

        private readonly IMetadataProvider _metadataProvider;
        private readonly ITypeProvider _typeProvider;

        public ControllerAssemblyBuilder(IMetadataProvider metadataProvider, ITypeProvider typeProvider)
        {
            _metadataProvider = metadataProvider;
            _typeProvider = typeProvider;
        }

        public Assembly BuildControllerAssembly(Uri uri)
        {
            BoundedContextElement boundedContextElement;
            if (!_metadataProvider.TryGetMetadata<BoundedContextElement>(uri, out boundedContextElement))
            {
                throw new ArgumentException();
            }

            return BuildControllerAssemblyInternal(boundedContextElement.ConceptualModel.Entities);
        }

        private Assembly BuildControllerAssemblyInternal(IEnumerable<EntityElement> entities)
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