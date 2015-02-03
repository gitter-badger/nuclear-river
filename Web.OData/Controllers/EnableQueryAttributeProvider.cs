using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.OData;
using System.Web.OData.Query;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.Web.OData.DynamicControllers;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public sealed class EnableQueryAttributeProvider : IFilterProvider
    {
        private readonly IMetadataProvider _metadataProvider;

        public EnableQueryAttributeProvider(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var queryOptionsParameter = actionDescriptor.GetParameters().SingleOrDefault(x => typeof(ODataQueryOptions).IsAssignableFrom(x.ParameterType));
            if (queryOptionsParameter == null)
            {
                return Enumerable.Empty<FilterInfo>();
            }

            var entityElementIdAttribute = actionDescriptor.ControllerDescriptor.GetCustomAttributes<EntityElementIdAttribute>().SingleOrDefault();
            if (entityElementIdAttribute == null)
            {
                return Enumerable.Empty<FilterInfo>();
            }

            var filter = CreateActionFilter(entityElementIdAttribute.Uri);

            return new[] { new FilterInfo(filter, FilterScope.Global) };
        }

        private IActionFilter CreateActionFilter(Uri entityElementId)
        {
            EntityElement entityElement;
            if (!_metadataProvider.TryGetMetadata(entityElementId, out entityElement))
            {
                throw new ArgumentException();
            }

            // далее можно кастомизовать EnableQueryAttribute используя entityElement, но пока это не нужно
            var enableQueryAttribute = new EnableQueryAttribute
            {
                PageSize = 40
            };

            return enableQueryAttribute;
        }
    }
}