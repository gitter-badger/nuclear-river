using System.Linq;
using System.Web.Http;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.DynamicControllers;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData
{
    internal sealed class ODataModelRegistrator
    {
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
                var contextName = contextId.Segments.Last();

                var edmModel = _edmModelWithClrTypesBuilder.Build(contextId);
                httpServer.Configuration.MapODataServiceRoute(contextName, contextName, edmModel, new DefaultODataBatchHandler(httpServer));

                _dynamicControllersRegistrator.RegisterDynamicControllers(contextId);
            }
        }
    }
}