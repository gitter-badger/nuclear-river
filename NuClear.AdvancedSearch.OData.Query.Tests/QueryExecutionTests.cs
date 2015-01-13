using System.Linq;
using System.Net.Http;
using System.Web.OData.Extensions;
using System.Web.OData.Query;

using AdvancedSearch.TestData;

using Newtonsoft.Json;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.OData.Query.Tests
{
    [TestFixture]
    public class QueryExecutionTests
    {
        [Test]
        public void Test_Filter()
        {
            var request = TestHelper.CreateRequest("$filter=Id ne 1");
            var queryOptions = CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Where(x => x.Id != 1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Count()
        {
            var request = TestHelper.CreateRequest("$count=true");
            var queryOptions = CreateQueryOptions(request);

            queryOptions.ApplyTo(Repositories.Class1);
            var actual = request.ODataProperties().TotalCount;
            var expected = Repositories.Class1.Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_OrderBy()
        {
            var request = TestHelper.CreateRequest("$orderby=Id desc");
            var queryOptions = CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.OrderByDescending(x => x.Id));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_SkipTop()
        {
            var request = TestHelper.CreateRequest("$skip=2&$top=1");
            var queryOptions = CreateQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Skip(2).Take(1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Select()
        {
            var request = TestHelper.CreateRequest("$select=Id");
            var queryOptions = CreateQueryOptions(request);

            var data = queryOptions.ApplyTo(Repositories.Class1);
            var actual = JsonConvert.SerializeObject(data);
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Select(x => new { x.Id }));

            Assert.AreEqual(expected, actual);
        }

        //[Test]
        //public void Test_Expand()
        //{
        //    var request = TestHelper.CreateRequest("$expand=Class2");
        //    var queryOptions = CreateQueryOptions(request);

        //    var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
        //    var expected = JsonConvert.SerializeObject(Repositories.Class1.Select(x => new
        //    {
        //        x.Id,
        //        Class2 = (x.Class2 == null) ? null : new { x.Class2.Id }
        //    }));

        //    Assert.AreEqual(expected, actual);
        //}

        private static ODataQueryOptions CreateQueryOptions(HttpRequestMessage request)
        {
            var model = Class1EdmModelBuilder.GetEdmModel();
            var elementClrType = typeof(Class1);

            var queryOptions = TestHelper.CreateQueryOptions(model, elementClrType, request);
            return queryOptions;
        }

        // TODO: paging
    }
}
