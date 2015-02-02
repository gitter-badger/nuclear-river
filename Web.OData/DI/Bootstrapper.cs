using System.Data.Entity.Infrastructure;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;

using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Emit;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.Controllers;
using NuClear.AdvancedSearch.Web.OData.Dynamic;
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
                .ConfigureStoreModel()
                .ConfigureWebApi();

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
            var providerInfo = new DbProviderInfo("System.Data.SqlClient", "2012");

            return container
                .RegisterType<ITypeProvider, EmitTypeProvider>(Lifetime.Singleton)
                .RegisterType<EdmxModelBuilder>(Lifetime.Singleton, new InjectionConstructor(providerInfo, typeof(IMetadataProvider), typeof(ITypeProvider)));
        }

        public static IUnityContainer ConfigureWebApi(this IUnityContainer container)
        {
            return container
                .RegisterType<IDynamicAssembliesRegistry, DynamicAssembliesRegistry>(Lifetime.Singleton)
                .RegisterType<IDynamicAssembliesResolver, DynamicAssembliesRegistry>(Lifetime.Singleton)

                // custom IHttpControllerTypeResolver
                .RegisterType<IHttpControllerTypeResolver, DynamicControllerTypeResolver>(Lifetime.Singleton)

                // custom IFilterProvider
                .RegisterType<IFilterProvider, EnableQueryAttributeProvider>(typeof(EnableQueryAttributeProvider).Name, Lifetime.Singleton)

                .RegisterType<IFinder, ODataFinder>(Lifetime.PerScope);
        }
    }
}