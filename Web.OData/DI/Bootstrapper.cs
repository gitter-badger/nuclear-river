using System.Net.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using System.Web.OData.Extensions;

using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.OData.Edm;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Emit;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.Controllers;
using NuClear.AdvancedSearch.Web.OData.DataAccess;
using NuClear.AdvancedSearch.Web.OData.DynamicControllers;
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
                .ConfigureWebApiOData();

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
            return container
                .RegisterType<ITypeProvider, EmitTypeProvider>(Lifetime.Singleton)
                .RegisterType<EdmxModelBuilder>(Lifetime.Singleton, new InjectionConstructor(typeof(IMetadataProvider), typeof(ITypeProvider)))
                .RegisterType<ODataConnectionFactory>(Lifetime.Singleton);
        }

        public static IUnityContainer ConfigureWebApiOData(this IUnityContainer container)
        {
            return container
                .RegisterType<IDynamicAssembliesRegistry, DynamicAssembliesRegistry>(Lifetime.Singleton)
                .RegisterType<IDynamicAssembliesResolver, DynamicAssembliesRegistry>(Lifetime.Singleton)

                // custom IHttpControllerTypeResolver
                .RegisterType<IHttpControllerTypeResolver, DynamicControllerTypeResolver>(Lifetime.Singleton)

                .RegisterType<IFinder, ODataFinder>(Lifetime.PerScope)
                .RegisterType<IEdmModel>(Lifetime.PerScope, new InjectionFactory(c => c.Resolve<HttpRequestMessage>().ODataProperties().Model));
        }
    }
}