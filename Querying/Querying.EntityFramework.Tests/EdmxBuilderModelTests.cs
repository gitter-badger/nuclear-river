using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using NuClear.Querying.EntityFramework.Tests.Model.CustomerIntelligence;

using NUnit.Framework;

namespace NuClear.Querying.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldQueryBusinessDirectoryModel()
        {
            var model = CreateModel(AdvancedSearchModel.CustomerIntelligence);
            model.Dump();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                Assert.That(context.Set<Project>().ToArray(), Has.Length.EqualTo(6));
                Assert.That(context.Set<Territory>().ToArray(), Has.Length.EqualTo(5));
                Assert.That(context.Set<CategoryGroup>().ToArray(), Has.Length.EqualTo(5));
            }
        }

        [Test]
        public void ShouldQueryCustomerIntelligenceModel()
        {
            var model = CreateModel(AdvancedSearchModel.CustomerIntelligence);
            model.Dump();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                Assert.That(context.Set<Client>().ToArray(), Has.Length.EqualTo(1));
                Assert.That(context.Set<ClientContact>().ToArray(), Has.Length.EqualTo(3));
                Assert.That(context.Set<Firm>().ToArray(), Has.Length.EqualTo(2));
                Assert.That(context.Set<FirmBalance>().ToArray(), Has.Length.EqualTo(3));
                Assert.That(context.Set<FirmCategory>().ToArray(), Has.Length.EqualTo(3));
            }
        }

        [Test]
        public void ShouldQueryClients()
        {
            var model = CreateModel(AdvancedSearchModel.CustomerIntelligence);

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
            var model = CreateModel(AdvancedSearchModel.CustomerIntelligence);

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                var firm = context.Set<Firm>()
                    .Include(x => x.Balances)
                    .Include(x => x.Categories)
                    .Include(x => x.CategoryGroup)
                    .Include(x => x.Client)
                    .Include(x => x.Client.CategoryGroup)
                    .Include(x => x.Client.Contacts)
                    .Include(x => x.Territory)
                    .OrderBy(x => x.Id)
                    .FirstOrDefault();

                Assert.That(firm, Is.Not.Null);
                Assert.That(firm.Name, Is.Not.Null.And.EqualTo("Firm 1"));
                Assert.That(firm.Balances, Is.Not.Null.And.Count.EqualTo(2));
                Assert.That(firm.Categories, Is.Not.Empty.And.Count.EqualTo(1));
                Assert.That(firm.CategoryGroup, Is.Not.Null);
                Assert.That(firm.Client, Is.Not.Null.And.Property("Name").EqualTo("Client 1"));
                Assert.That(firm.Client.CategoryGroup, Is.Not.Null);
                Assert.That(firm.Client.Contacts, Is.Not.Null.And.Count.EqualTo(3));
                Assert.That(firm.Territory, Is.Not.Null);
            }
        }

        [Test]
        public void ShouldQueryFirmsViaCustomModel()
        {
            var builder = new DbModelBuilder();
            builder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            builder.Conventions.Remove<PluralizingTableNameConvention>();

            builder.Entity<Firm>().ToTable("FirmView");
            builder.Entity<FirmBalance>().HasKey(x => new { x.AccountId, x.FirmId });
            builder.Entity<FirmCategory>().HasKey(x => new { x.CategoryId, x.FirmId });
            builder.Entity<ClientContact>().HasKey(x => new { x.ContactId, x.ClientId });

            var model = builder.Build(EffortProvider);
            model.Dump();

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                var firm = context.Set<Firm>()
                    .Include(x => x.Balances)
                    .Include(x => x.Categories)
                    .OrderBy(x => x.Id)
                    .FirstOrDefault();
                
                Assert.That(firm, Is.Not.Null);
                Assert.That(firm.Name, Is.Not.Null.And.EqualTo("Firm 1"));
                Assert.That(firm.Balances, Is.Not.Null.And.Count.EqualTo(2));
                Assert.That(firm.Categories, Is.Not.Null.And.Count.EqualTo(1));
            }
        }

        private static DbModel CreateModel(AdvancedSearchModel model)
        {
            return BuildModel(AdvancedSearchMetadataSource, model, CustomerIntelligenceTypeProvider);
        }
  }
}