using System.Collections.Generic;
using System.Reflection;

namespace NuClear.Querying.Web.OData.DynamicControllers
{
    public interface IDynamicAssembliesResolver
    {
        ICollection<Assembly> GetDynamicAssemblies();
    }

    public interface IDynamicAssembliesRegistry : IDynamicAssembliesResolver
    {
        void RegisterDynamicAssembly(Assembly assembly);
    }

    public sealed class DynamicAssembliesRegistry : IDynamicAssembliesRegistry
    {
        private readonly List<Assembly> _dynamicAssemblies = new List<Assembly>();

        public void RegisterDynamicAssembly(Assembly assembly)
        {
            _dynamicAssemblies.Add(assembly);
        }

        public ICollection<Assembly> GetDynamicAssemblies()
        {
            return _dynamicAssemblies;
        }
    }
}