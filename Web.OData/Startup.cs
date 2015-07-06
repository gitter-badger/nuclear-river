using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.Cors;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Web.OData;
using NuClear.AdvancedSearch.Web.OData.DataAccess;
using NuClear.AdvancedSearch.Web.OData.DI;
using NuClear.AdvancedSearch.Web.OData.Settings;

using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace NuClear.AdvancedSearch.Web.OData
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var httpServer = new HttpServer(new HttpConfiguration());

            var settingsContainer = new WebApplicationSettings();

            // DI
            var container = Bootstrapper.ConfigureUnity(settingsContainer);
            httpServer.Configuration.DependencyResolver = new UnityResolver(container);

            // turn on CORS support
            httpServer.Configuration.EnableCors(new EnableCorsAttribute("*", "*", "*"));

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