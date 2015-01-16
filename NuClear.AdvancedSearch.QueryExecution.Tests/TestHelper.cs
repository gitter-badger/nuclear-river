using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Net.Http;
using System.Web.OData;
using System.Web.OData.Query;

using Effort;

using Microsoft.OData.Edm;

using Newtonsoft.Json.Linq;

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

        public static DbContext CreateInMemoryContext(this DbCompiledModel dbCompiledModel)
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new DbContext(connection, dbCompiledModel, true);
            return context;
        }

        public static object EsqlQuery(this IObjectContextAdapter objectContextAdapter, string commandText)
        {
            var objectContext = objectContextAdapter.ObjectContext;
            var objectQuery = new ObjectQuery<DbDataRecord>(commandText, objectContext);

            var jArray = new JArray();
            foreach (var dataRecord in objectQuery)
            {
                var jObject = new JObject();
                for (var i = 0; i < dataRecord.FieldCount; i++)
                {
                    jObject.Add(dataRecord.GetName(i), new JValue(dataRecord.GetValue(i)));
                }

                jArray.Add(jObject);
            }

            return jArray;
        }
    }
}