using System.Data.Entity;
using System.Web.Http;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Web.OData;
using NuClear.AdvancedSearch.Web.OData.DataAccess;
using NuClear.AdvancedSearch.Web.OData.DI;

using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace NuClear.AdvancedSearch.Web.OData
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var httpServer = new HttpServer(new HttpConfiguration());

            // DI
            var container = Bootstrapper.ConfigureUnity();
            httpServer.Configuration.DependencyResolver = new UnityResolver(container).RegisterHttpRequestMessage(httpServer.Configuration);

            // default web api
            httpServer.Configuration.MapHttpAttributeRoutes();
            httpServer.Configuration.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            // configure entity framework
            DbConfiguration.SetConfiguration(new ODataDbConfiguration());

            // register odata models
            var modelRegistrator = container.Resolve<ODataModelRegistrator>();
            modelRegistrator.RegisterModels(httpServer);

            appBuilder.UseWebApi(httpServer);
        }
    }
}