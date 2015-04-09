using Microsoft.Practices.Unity;

namespace NuClear.AdvancedSearch.Messaging.Tests.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container)
        {
            return container;
        }
    }
}
