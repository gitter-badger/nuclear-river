using System;
using System.Data.Entity;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence;

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
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=OrganizationUnit/Id eq 0");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.OrganizationUnit.Id == 0);

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ByLastDisqualifiedOn()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=LastDisqualifiedOn gt 2015-01-01");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.LastDisqualifiedOn > new DateTimeOffset(2015, 01, 01, 00, 00, 00, TimeSpan.Zero));

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ByClient()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=Client/CategoryGroup/Id eq 0");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.CategoryGroup.Id == 0);

                var test = context.Set<Firm>().Select(x => new
                                                           {
                                                               x,
                                                               test = x.CategoryGroup
                                                           }).ToArray();

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ByCategory()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=Categories/any(x: x/CategoryGroup/Id eq 0)");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.Categories.Any(y => y.CategoryGroup.Id == 0));

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ByAccount()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=Client/Accounts/all(x: x/Balance gt 0.01M)");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.Client.Accounts.All(y => y.Balance > 0.01M));

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ByContact()
        {
            using (var context = new DbContext(CreateConnection(), Model, true))
            {
                var queryOptions = CreateValidQueryOptions<Firm>("$filter=Client/Contacts/any(x: x/Role eq AdvancedSearch.CustomerIntelligence.ContactRole'Employee')");

                var actual = queryOptions.ApplyTo(context.Set<Firm>());
                var expected = context.Set<Firm>().Where(x => x.Client.Contacts.Any(y => y.Role == ContactRole.Employee));

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}