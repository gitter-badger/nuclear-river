using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.AdvancedSearch.Web.OData.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity()
        {
            var container = new UnityContainer()
                .ConfigureMetadata();

            return container;
        }

        public static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            var metadataSource = new AdvancedSearchMetadataSource();
            var metadataProvider = new MetadataProvider(new IMetadataSource[] { metadataSource }, new IMetadataProcessor[] { });
            return container.RegisterInstance<IMetadataProvider>(metadataProvider, Lifetime.Singleton);
        }
    }
}