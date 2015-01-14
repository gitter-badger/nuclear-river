using System;
using System.Net.Http;
using System.Web.OData;
using System.Web.OData.Query;

using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    public static class TestHelper
    {
        public static ODataQueryOptions CreateValidQueryOptions(IEdmModel model, Type elementClrType, HttpRequestMessage request, ODataValidationSettings validationSettings)
        {
            var context = new ODataQueryContext(model, elementClrType, null);
            var queryOptions = new ODataQueryOptions(context, request);
            queryOptions.Validate(validationSettings);

            return queryOptions;
        }

        public static HttpRequestMessage CreateRequest(string query = null)
        {
            var uriBuilder = new UriBuilder();
            if (query != null)
            {
                uriBuilder.Query = query;
            }

            return new HttpRequestMessage { RequestUri = uriBuilder.Uri };
        }
    }
}