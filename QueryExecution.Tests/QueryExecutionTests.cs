using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
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
            var entities = Enumerable.Range(0, 10).Select(x => new TestClass1
            {
                Id = x,
                Name = x.ToString(),
                TestClass2 = new TestClass2
                {
                    Id = x,
                    Name = x.ToString()
                }
            });
            _context.Set<TestClass1>().AddRange(entities);

            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                
            }
            
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Test]
        public void Test_Filter()
        {
            var queryOptions = CreateValidQueryOptions("$filter=Id ne 1");

            var actual = queryOptions.ApplyTo(_query);
            var expected = _query.Where(x => x.Id != 1);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Test_Filter_Enum()
        {
            var queryOptions = CreateValidQueryOptions("$filter=Enum1 eq AdvancedSearch.Context.Enum1'Member1'");

            var actual = queryOptions.ApplyTo(_query);
            var expected = _query.Where(x => x.Enum1 == Enum1.Member1);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Filter_Negative()
        {
            CreateValidQueryOptions("$filter=NonExistent ne null");
        }

        [Test, Ignore("https://github.com/tamasflamich/effort/issues/6")]
        public void Test_Count()
        {
            var request = TestHelper.CreateRequest();
            var queryOptions = CreateValidQueryOptions("$count=true");

            queryOptions.ApplyTo(_query);
            var actual = request.ODataProperties().TotalCount;
            var expected = ((IEnumerable)_query).OfType<object>().Count();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Test_OrderBy()
        {
            var queryOptions = CreateValidQueryOptions("$orderby=Id desc");

            var actual = queryOptions.ApplyTo(_query);
            var expected = _query.OrderByDescending(x => x.Id);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_OrderBy_Negative()
        {
            CreateValidQueryOptions("$orderby=NonExistent desc");
        }

        [Test]
        public void Test_SkipTop()
        {
            var queryOptions = CreateValidQueryOptions("$skip=2&$top=1");

            var actual = queryOptions.ApplyTo(_query);
            var expected = _query.OrderBy(x => x.Id).Skip(2).Take(1);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Test_Select()
        {
            var queryOptions = CreateValidQueryOptions("$select=Id,Name");

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.Select(x => new { x.Name, x.Id }));

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Select_Negative()
        {
            CreateValidQueryOptions("$select=NonExistent");
        }

        [Test]
        public void Test_Expand()
        {
            var queryOptions = CreateValidQueryOptions("$select=TestClass2&$expand=TestClass2");

            var actual = JsonConvert.SerializeObject(queryOptions.ApplyTo(_query));
            var expected = JsonConvert.SerializeObject(_query.Select(x => new { x.TestClass2 }));

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [ExpectedException(typeof(ODataException))]
        public void Test_Expand_Negative()
        {
            CreateValidQueryOptions("$expand=NonExistent");
        }

        [Test]
        public void Test_Paging()
        {
            var queryOptions = CreateValidQueryOptions();
            var querySettings = new ODataQuerySettings
            {
                PageSize = 2
            };

            var actual = queryOptions.ApplyTo(_query, querySettings);
            var expected = _query.Take(2);

            Assert.That(actual, Is.EqualTo(expected));
        }

        public ODataQueryOptions CreateValidQueryOptions(string query = null)
        {
            var request = TestHelper.CreateRequest(query);
            var queryOptions = TestHelper.CreateValidQueryOptions(TestModel.EdmModel, typeof(TestClass1), request, new ODataValidationSettings());
            return queryOptions;
        }
    }
}
