using System.Linq;
using System.Net.Http;
using System.Web.OData.Extensions;
using System.Web.OData.Query;

using Microsoft.OData.Core;

using Newtonsoft.Json;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    [TestFixture]
    public class QueryExecutionTests
    {
        [Test]
        public void Test_Filter()
        {
            var request = TestHelper.CreateRequest("$filter=Id ne 1");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Where(x => x.Id != 1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Filter_Negative()
        {
            var request = TestHelper.CreateRequest("$filter=NonExistent ne null");
            CreateValidQueryOptions(request);
        }

        [Test]
        public void Test_Count()
        {
            var request = TestHelper.CreateRequest("$count=true");
            var queryOptions = CreateValidQueryOptions(request);

            queryOptions.ApplyTo(Repositories.Class1);
            var actual = request.ODataProperties().TotalCount;
            var expected = Repositories.Class1.Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_OrderBy()
        {
            var request = TestHelper.CreateRequest("$orderby=Id desc");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.OrderByDescending(x => x.Id));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_OrderBy_Negative()
        {
            var request = TestHelper.CreateRequest("$orderby=NonExistent desc");
            CreateValidQueryOptions(request);
        }

        [Test]
        public void Test_SkipTop()
        {
            var request = TestHelper.CreateRequest("$skip=2&$top=1");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Skip(2).Take(1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Select()
        {
            var request = TestHelper.CreateRequest("$select=Id");
            var queryOptions = CreateValidQueryOptions(request);

            var data = queryOptions.ApplyTo(Repositories.Class1);
            var actual = JsonConvert.SerializeObject(data);
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Select(x => new { x.Id }));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Select_Negative()
        {
            var request = TestHelper.CreateRequest("$select=NonExistent");
            CreateValidQueryOptions(request);
        }


        [Test]
        public void Test_Expand()
        {
            var request = TestHelper.CreateRequest("$expand=TestClass2");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Select(x => new
            {
                TestClass2 = (x.TestClass2 == null) ? null : new { x.TestClass2.Id },
                x.Id
            }));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Expand_Negative()
        {
            var request = TestHelper.CreateRequest("$expand=NonExistent");
            CreateValidQueryOptions(request);
        }

        [Test]
        public void Test_Paging()
        {
            var request = TestHelper.CreateRequest();
            var queryOptions = CreateValidQueryOptions(request);
            var querySettings = new ODataQuerySettings
            {
                PageSize = 2
            };

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(Repositories.Class1, querySettings));
            var expected = JsonConvert.SerializeObject(Repositories.Class1.Take(2));

            Assert.AreEqual(expected, actual);
        }

        private static ODataQueryOptions CreateValidQueryOptions(HttpRequestMessage request)
        {
            var model = Class1EdmModelBuilder.GetEdmModel();
            var elementClrType = typeof(TestClass1);

            var queryOptions = TestHelper.CreateValidQueryOptions(model, elementClrType, request, new ODataValidationSettings());
            return queryOptions;
        }
    }
}
