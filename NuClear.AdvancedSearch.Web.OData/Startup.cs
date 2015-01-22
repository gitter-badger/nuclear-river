using System.Web.Http;
using System.Web.OData.Extensions;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.OData;
using NuClear.AdvancedSearch.Web.OData;
using NuClear.AdvancedSearch.Web.OData.DI;

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

            //config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var edmModelFactory = container.Resolve<EdmModelFactory>();
            var edmModel = edmModelFactory.Create("CustomerIntelligence");

            config.MapODataServiceRoute("CustomerIntelligence", "CustomerIntelligence", edmModel);

            appBuilder.UseWebApi(config);
        }
    }
}