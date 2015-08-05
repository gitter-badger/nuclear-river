using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.OData;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.DynamicControllers;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public sealed class DynamicEnableQueryAttribute : EnableQueryAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var entityElementIdAttribute = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<EntityElementIdAttribute>().SingleOrDefault();
            if (entityElementIdAttribute == null)
            {
                throw new ArgumentException();
            }

            var dependencyResolver = actionExecutedContext.Request.GetConfiguration().DependencyResolver;
            var metadataProvider = (IMetadataProvider)dependencyResolver.GetService(typeof(IMetadataProvider));

            Customize(metadataProvider, entityElementIdAttribute.Uri);

            base.OnActionExecuted(actionExecutedContext);
        }

        private void Customize(IMetadataProvider metadataProvider, Uri entityElementId)
        {
            EntityElement entityElement;
            if (!metadataProvider.TryGetMetadata(entityElementId, out entityElement))
            {
                throw new ArgumentException();
            }

            // ограничение для серверного paging
            PageSize = 100;
        }
    }
}