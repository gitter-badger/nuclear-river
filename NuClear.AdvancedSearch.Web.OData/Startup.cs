using System.Web.Http;
using System.Web.OData.Extensions;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData;
using NuClear.AdvancedSearch.Web.OData.DI;
using NuClear.Metamodeling.Elements.Identities;

using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace NuClear.AdvancedSearch.Web.OData
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            // DI
            var container = Bootstrapper.ConfigureUnity();
            config.DependencyResolver = new UnityResolver(container);

            // default web api
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            ConfigureODATAContexts(config, container);

            appBuilder.UseWebApi(config);
        }

        public void ConfigureODATAContexts(HttpConfiguration config, IUnityContainer container)
        {
            var uri = IdBuilder.For<AdvancedSearchIdentity>("CustomerIntelligence");

            var typedEdmModelBuilder = container.Resolve<TypedEdmModelBuilder>();
            var edmModel = typedEdmModelBuilder.Build(uri);
            config.MapODataServiceRoute("CustomerIntelligence", "CustomerIntelligence", edmModel);

            var registrator = container.Resolve<DynamicControllersRegistrator>();
            registrator.RegisterDynamicControllers(uri);
        }
    }
}