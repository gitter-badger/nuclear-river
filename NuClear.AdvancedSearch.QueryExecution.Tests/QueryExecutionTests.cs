using System;
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
        private Type _testType;
        private DbContext _context;
        private DbSet _testDbSet;

        [SetUp]
        public void Init()
        {
            _testType = TestModel.EdmModel.GetClrTypes().Single(x => string.Equals(x.Name, "TestClass1"));
            _context = TestModel.EFModel.CreateInMemoryContext();

            // insert test data
            _testDbSet = _context.Set(_testType);
            var idProperty = _testType.GetProperty("Id");
            for (var i = 0; i < 10; i++)
            {
                var @object = Activator.CreateInstance(_testType);
                idProperty.SetValue(@object, i);
                _testDbSet.Add(@object);
            }
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

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_testDbSet));
            var expected = JsonConvert.SerializeObject(_context.EsqlQuery("SELECT T.Id FROM TestClass1 AS T WHERE T.Id <> 1"));

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

            queryOptions.ApplyTo(_testDbSet);
            var actual = request.ODataProperties().TotalCount;
            var expected = ((IEnumerable)_testDbSet).OfType<object>().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_OrderBy()
        {
            var request = TestHelper.CreateRequest("$orderby=Id desc");
            var queryOptions = CreateValidQueryOptions(request);

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_testDbSet));
            var expected = JsonConvert.SerializeObject(_context.EsqlQuery("SELECT T.Id FROM TestClass1 AS T ORDER BY T.Id DESC"));

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

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_testDbSet));
            var expected = JsonConvert.SerializeObject(_context.EsqlQuery("SELECT T.Id FROM TestClass1 AS T ORDER BY T.Id SKIP 2 LIMIT 1"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Select()
        {
            var request = TestHelper.CreateRequest("$select=Id");
            var queryOptions = CreateValidQueryOptions(request);

            var data = queryOptions.ApplyTo(_testDbSet);
            var actual = JsonConvert.SerializeObject(data);
            var expected = JsonConvert.SerializeObject(_context.EsqlQuery("SELECT T.Id FROM TestClass1 AS T"));

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

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_testDbSet));
            var expected = JsonConvert.SerializeObject(_context.EsqlQuery("SELECT T.Id FROM TestClass1 AS T"));

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

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_testDbSet, querySettings));
            var expected = JsonConvert.SerializeObject(_context.EsqlQuery("SELECT TOP(2) T.Id FROM TestClass1 AS T"));

            Assert.AreEqual(expected, actual);
        }

        private ODataQueryOptions CreateValidQueryOptions(HttpRequestMessage request)
        {
            var queryOptions = TestHelper.CreateValidQueryOptions(TestModel.EdmModel, _testType, request, new ODataValidationSettings());
            return queryOptions;
        }
    }
}
