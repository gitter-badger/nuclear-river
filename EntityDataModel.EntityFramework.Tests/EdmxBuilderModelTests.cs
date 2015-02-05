using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldQueryData()
        {
            var model = CreateCustomerIntelligenceModel();
            model.Dump();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                Assert.That(context.Set<Account>().ToArray(), Has.Length.EqualTo(1));
                Assert.That(context.Set<Category>().ToArray(), Has.Length.EqualTo(5));
                Assert.That(context.Set<Client>().ToArray(), Has.Length.EqualTo(1));
                Assert.That(context.Set<Contact>().ToArray(), Has.Length.EqualTo(3));
                Assert.That(context.Set<Firm>().ToArray(), Has.Length.EqualTo(2));
                Assert.That(context.Set<FirmCategory>().ToArray(), Has.Length.EqualTo(10));
                Assert.That(context.Set<OrganizationUnit>().ToArray(), Has.Length.EqualTo(6));
                Assert.That(context.Set<Territory>().ToArray(), Has.Length.EqualTo(5));
            }
        }

        [Test]
        public void ShouldQueryClients()
        {
            var model = CreateCustomerIntelligenceModel();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                var clients = context.Set<Client>().Include(x => x.Contacts).ToArray();

                Assert.That(clients, Has.Length.EqualTo(1));
                Assert.That(clients.First().Contacts, Has.Count.EqualTo(3));
            }
        }

        [Test]
        public void ShouldQueryFirms()
        {
            var model = CreateCustomerIntelligenceModel();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                var firms = context.Set<Firm>()
                    .Include(x => x.OrganizationUnit)
                    .Include(x => x.Territory)
                    .Include(x => x.Categories)
                    .Include(x => x.Client)
                    .Include(x => x.Client.Accounts)
                    .ToList();

                Assert.That(firms, Has.Count.EqualTo(2));
                Assert.That(firms.First().OrganizationUnit, Is.Not.Null.And.Property("Name").EqualTo("Новосибирск"));
                Assert.That(firms.First().Territory, Is.Not.Null.And.Property("Name").EqualTo("Новосибирск.Региональная территория"));
                Assert.That(firms.First().Categories, Has.Count.EqualTo(10));
                Assert.That(firms.Last().Categories, Has.Count.EqualTo(0));
                Assert.That(firms.First().Client, Is.Not.Null);
                Assert.That(firms.Last().Client, Is.Not.Null);
                Assert.That(firms.First().Client.Accounts, Has.Count.EqualTo(1));
            }
        }

        [Test, Explicit]
        public void ShouldQueryFirmsForCustomModel()
        {
            var builder = new DbModelBuilder();
            builder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            builder.Conventions.Remove<PluralizingTableNameConvention>();

            builder.Entity<Firm>();

            var model = builder.Build(EffortProvider);
            model.Dump();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                var firms = context.Set<Firm>().ToArray();
            }
        }

        private static DbModel CreateCustomerIntelligenceModel()
        {
            return BuildModel(CustomerIntelligenceMetadataSource, CustomerIntelligenceTypeProvider);
        }
  }
}