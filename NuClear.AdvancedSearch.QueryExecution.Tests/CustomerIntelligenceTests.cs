using System;
using System.Data.Entity;
using System.Linq;

using EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    [TestFixture]
    public sealed class CustomerIntelligenceTests : CustomerIntelligenceBaseFixture
    {
        [Test]
        public void ByOrganizationUnit()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=OrganizationUnitId eq 0");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.OrganizationUnitId == 0);

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test, Ignore("Should be fixed in version 5.4 (https://github.com/OData/WebApi/issues/136)")]
        public void ByLastQualifiedOn()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=LastQualifiedOn gt datetime'2015-01-01T00:00'");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.LastQualifiedOn > new DateTime(2015, 01, 01));

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ByContact()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=HasPhone eq true or HasWebsite eq true");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.HasPhone || x.HasWebsite);

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}