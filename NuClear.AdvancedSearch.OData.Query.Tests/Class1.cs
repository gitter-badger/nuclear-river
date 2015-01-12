using System;
using System.Linq;
using System.Net.Http;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Query;

using Microsoft.OData.Edm;

using Newtonsoft.Json;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.OData.Query.Tests
{
    public static class Repositories
    {
        public static IQueryable<Class1> Class1 = new[]
                                         {
                                             new Class1 { Id = 1 },
                                             new Class1 { Id = 2, Class2 = new Class2 { Id = 0 } },
                                             new Class1 { Id = 3 },
                                             new Class1 { Id = 4, Class2 = new Class2 { Id = 1 } },
                                         }.AsQueryable();

    }

    public sealed class Class1
    {
        public int Id { get; set; }
        public Class2 Class2 { get; set; }
    }

    public sealed class Class2
    {
        public int Id { get; set; }
    }

    public static class Class1EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Class1>("Class1");
            builder.EntitySet<Class2>("Class2");

            return builder.GetEdmModel();
        }
    }

    public static class TestHelper
    {
        public static ODataQueryOptions CreateQueryOptions(HttpRequestMessage request)
        {
            var model = Class1EdmModelBuilder.GetEdmModel();
            var elementClrType = typeof(Class1);

            var queryOptions = CreateQueryOptions(model, elementClrType, request);
            return queryOptions;
        }

        public static ODataQueryOptions CreateQueryOptions(IEdmModel model, Type elementClrType, HttpRequestMessage request)
        {
            var context = new ODataQueryContext(model, elementClrType, null);
            var queryOptions = new ODataQueryOptions(context, request);
            return queryOptions;
        }

        public static HttpRequestMessage CreateRequest(string query)
        {
            var uriBuilder = new UriBuilder { Query = query };
            return new HttpRequestMessage { RequestUri = uriBuilder.Uri };
        }
    }

    [TestFixture]
    public class Class1Tests
    {
        [Test]
        public void Test_Filter()
        {
            var request = TestHelper.CreateRequest("$filter=Id ne 1");
            var queryOptions = TestHelper.CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Where(x => x.Id != 1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Count()
        {
            var request = TestHelper.CreateRequest("$count=true");
            var queryOptions = TestHelper.CreateQueryOptions(request);

            queryOptions.ApplyTo(Repositories.Class1);
            var actual = request.ODataProperties().TotalCount;
            var expected = Repositories.Class1.Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_OrderBy()
        {
            var request = TestHelper.CreateRequest("$orderby=Id desc");
            var queryOptions = TestHelper.CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.OrderByDescending(x => x.Id));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_SkipTop()
        {
            var request = TestHelper.CreateRequest("$skip=2&$top=1");
            var queryOptions = TestHelper.CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Skip(2).Take(1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_SelectExpand()
        {
            var request = TestHelper.CreateRequest("$select=Id&$expand=Class2");
            var queryOptions = TestHelper.CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Select(x => new
            {
                x.Id,
                Class2 = (x.Class2 == null) ? null : new { x.Class2.Id }
            }));

            Assert.AreEqual(expected, actual);
        }

        // TODO: paging
    }
}
