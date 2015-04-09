using Microsoft.Practices.Unity;

namespace NuClear.AdvancedSearch.ServiceBus.Tests.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container)
        {
            return container;
        }
    }
}
