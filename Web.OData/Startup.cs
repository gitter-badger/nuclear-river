using System.Data.Entity;
using System.Web.Http;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData;
using NuClear.AdvancedSearch.Web.OData.DI;
using NuClear.AdvancedSearch.Web.OData.Dynamic;
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

            // per request DI
            UnityResolver.RegisterHttpRequestMessage(config);

            var dbConfiguration = container.Resolve<TestDbConfiguration>();
            DbConfiguration.SetConfiguration(dbConfiguration);

            // default web api
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var httpServer = new HttpServer(config);
            MapODataServiceRoute("CustomerIntelligence", httpServer, container);
            appBuilder.UseWebApi(httpServer);
        }

        public void MapODataServiceRoute(string contextName, HttpServer httpServer, IUnityContainer container)
        {
            var uri = IdBuilder.For<AdvancedSearchIdentity>(contextName);

            var edmModelBuilder = container.Resolve<EdmModelWithClrTypesBuilder>();
            var edmModel = edmModelBuilder.Build(uri);

            httpServer.Configuration.MapODataServiceRoute(contextName, contextName, edmModel, new DefaultODataBatchHandler(httpServer));

            var registrator = container.Resolve<DynamicControllersRegistrator>();
            registrator.RegisterDynamicControllers(uri);
        }
    }
}