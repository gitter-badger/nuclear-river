using System.Data.Entity.Infrastructure;

using DoubleGis.Erm.Platform.DI.Common.Config;

using Effort.Provider;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
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
                .ConfigureMetadata()
                .ConfigureStoreModel();

            return container;
        }

        public static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            var metadataSources = new IMetadataSource[]
            {
                new AdvancedSearchMetadataSource()
            };

            var metadataProcessors = new IMetadataProcessor[] { };

            return container.RegisterType<IMetadataProvider, MetadataProvider>(Lifetime.Singleton, new InjectionConstructor(metadataSources, metadataProcessors));
        }

        public static IUnityContainer ConfigureStoreModel(this IUnityContainer container)
        {
            EffortProviderConfiguration.RegisterProvider();

            var effortProviderInfo = new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
            return container.RegisterType<EdmxModelBuilder>(Lifetime.Singleton, new InjectionConstructor(effortProviderInfo, typeof(IMetadataProvider)));
        }
    }
}