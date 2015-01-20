using System.Collections;
using System.Data.Entity;
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
    public sealed class QueryExecutionTests
    {
        private DbContext _context;
        private IQueryable<TestClass1> _query;

        [SetUp]
        public void Init()
        {
            _context = TestModel.EFModel.CreateInMemoryContext();
            _query = _context.Set<TestClass1>();

            // insert test data
            var entities = Enumerable.Range(0, 10).Select(x => new TestClass1 { Id = x });
            _context.Set<TestClass1>().AddRange(entities);

            _context.SaveChanges();
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Test]
        public void Test_Filter()
        {
            var request = TestHelper.CreateRequest("$filter=Id ne 1");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.Where(x => x.Id != 1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Filter_Enum()
        {
            var request = TestHelper.CreateRequest("$filter=Enum1 eq AdvancedSearch.Context.Enum1'Member1'");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.Where(x => x.Enum1 == Enum1.Member1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Filter_Negative()
        {
            var request = TestHelper.CreateRequest("$filter=NonExistent ne null");
            CreateValidQueryOptions(request);
        }

        [Test, Ignore("https://github.com/tamasflamich/effort/issues/6")]
        public void Test_Count()
        {
            var request = TestHelper.CreateRequest("$count=true");
            var queryOptions = CreateValidQueryOptions(request);

            queryOptions.ApplyTo(_query);
            var actual = request.ODataProperties().TotalCount;
            var expected = ((IEnumerable)_query).OfType<object>().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_OrderBy()
        {
            var request = TestHelper.CreateRequest("$orderby=Id desc");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.OrderByDescending(x => x.Id));

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

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.OrderBy(x => x.Id).Skip(2).Take(1));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Select()
        {
            var request = TestHelper.CreateRequest("$select=Id,Name");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.Select(x => new { x.Name, x.Id }));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Select_Negative()
        {
            var request = TestHelper.CreateRequest("$select=NonExistent");
            CreateValidQueryOptions(request);
        }

        [Test, Ignore("Актализировать когда будет поддержка relations")]
        public void Test_Expand()
        {
            var request = TestHelper.CreateRequest("$expand=TestClass2");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(null);

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

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query, querySettings));
            var expected = JsonConvert.SerializeObject(_query.Take(2));

            Assert.AreEqual(expected, actual);
        }

        private static ODataQueryOptions CreateValidQueryOptions(HttpRequestMessage request)
        {
            var queryOptions = TestHelper.CreateValidQueryOptions(TestModel.EdmModel, typeof(TestClass1), request, new ODataValidationSettings());
            return queryOptions;
        }
    }
}
