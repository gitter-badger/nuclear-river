using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.DI;
using NuClear.AdvancedSearch.Web.OData.DynamicControllers;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData
{
    internal sealed class ODataModelRegistrator
    {
        private static readonly ConfigureHttpRequest ConfigureHttpRequest = Bootstrapper.ConfigureHttpRequest;

        private readonly IMetadataProvider _metadataProvider;
        private readonly DynamicControllersRegistrator _dynamicControllersRegistrator;
        private readonly EdmModelWithClrTypesBuilder _edmModelWithClrTypesBuilder;

        public ODataModelRegistrator(IMetadataProvider metadataProvider, DynamicControllersRegistrator dynamicControllersRegistrator, EdmModelWithClrTypesBuilder edmModelWithClrTypesBuilder)
        {
            _metadataProvider = metadataProvider;
            _dynamicControllersRegistrator = dynamicControllersRegistrator;
            _edmModelWithClrTypesBuilder = edmModelWithClrTypesBuilder;
        }

        public void RegisterModels(HttpServer httpServer)
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<AdvancedSearchIdentity>(out metadataSet))
            {
                return;
            }

            var contexts = metadataSet.Metadata.Values.OfType<BoundedContextElement>();
            foreach (var context in contexts)
            {
                var contextId = context.Identity.Id;
                var edmModel = _edmModelWithClrTypesBuilder.Build(contextId);

                var routePrefix = contextId.Segments.Last();
                MapRoute(routePrefix, edmModel, httpServer, ConfigureHttpRequest);

                _dynamicControllersRegistrator.RegisterDynamicControllers(contextId);
            }
        }

        private static void MapRoute(string routePrefix, IEdmModel edmModel, HttpServer httpServer, ConfigureHttpRequest configureHttpRequest)
        {
            // batch handler should be mapped first
            var batchHandler = new DefaultODataBatchHandler(httpServer) { ODataRouteName = routePrefix };
            var routeTemplate = string.IsNullOrEmpty(routePrefix) ? ODataRouteConstants.Batch : (routePrefix + '/' + ODataRouteConstants.Batch);

            var config = httpServer.Configuration;
            config.Routes.MapHttpBatchRoute(routePrefix + "Batch", routeTemplate, batchHandler);

            var additionalHandlers = new[]
            {
                new UnityResolver.PerRequestResolver(configureHttpRequest)
            };
            var handler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), additionalHandlers);
            config.MapODataServiceRoute(routePrefix, routePrefix, edmModel, handler);
        }
    }
}