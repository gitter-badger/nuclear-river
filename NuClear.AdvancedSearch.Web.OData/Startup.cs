using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.OData.Extensions;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.AdvancedSearch.Web.OData;
using NuClear.AdvancedSearch.Web.OData.Controllers;
using NuClear.AdvancedSearch.Web.OData.DI;
using NuClear.AdvancedSearch.Web.OData.Model;
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

            ConfigureODATA(config, container);

            appBuilder.UseWebApi(config);
        }

        public void ConfigureODATA(HttpConfiguration config, IUnityContainer container)
        {
            var uri = IdBuilder.For<AdvancedSearchIdentity>("CustomerIntelligence");

            var typedEdmModelBuilder = container.Resolve<TypedEdmModelBuilder>();
            var edmModel = typedEdmModelBuilder.Build(uri);
            config.MapODataServiceRoute("CustomerIntelligence", "CustomerIntelligence", edmModel);

            // TODO: remove
            var test = container.Resolve<ControllerAssemblyBuilder>();
            test.BuildControllerAssembly(uri);
        }
    }
}