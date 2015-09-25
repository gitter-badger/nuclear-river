using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dispatcher;

namespace NuClear.AdvancedSearch.Web.OData.DynamicControllers
{
    public sealed class DynamicControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        private readonly IDynamicAssembliesResolver _dynamicAssembliesResolver;

        public DynamicControllerTypeResolver(IDynamicAssembliesResolver dynamicAssembliesResolver)
        {
            _dynamicAssembliesResolver = dynamicAssembliesResolver;
        }

        public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
        {
            var controllerTypes = base.GetControllerTypes(assembliesResolver);

            var dynamicControllerTypes = _dynamicAssembliesResolver
                .GetDynamicAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => IsControllerTypePredicate(x));

            foreach (var dynamicControllerType in dynamicControllerTypes)
            {
                controllerTypes.Add(dynamicControllerType);
            }

            return controllerTypes;
        }
    }
}