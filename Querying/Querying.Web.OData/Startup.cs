using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.Cors;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.Querying.Web.OData;
using NuClear.Querying.Web.OData.DataAccess;
using NuClear.Querying.Web.OData.DI;
using NuClear.Querying.Web.OData.Settings;

using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace NuClear.Querying.Web.OData
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            // DI
            var settingsContainer = new WebApplicationSettings();
            var container = Bootstrapper.ConfigureUnity(settingsContainer);
            config.DependencyResolver = new UnityResolver(container);

            // turn on CORS support
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // default web api
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            // configure entity framework
            DbConfiguration.SetConfiguration(new ODataDbConfiguration());

            // register odata models
            var httpServer = new HttpServer(config);
            var modelRegistrator = container.Resolve<ODataModelRegistrator>();
            modelRegistrator.RegisterModels(httpServer);

            appBuilder.UseWebApi(httpServer);
        }
    }
}