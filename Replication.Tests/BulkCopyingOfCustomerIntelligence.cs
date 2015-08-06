using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    [TestFixture, Explicit("It's used to copy the data in bulk.")]
    internal class BulkCopyingOfCustomerIntelligence : BaseDataFixture
    {
        [Test]
        public void ReloadFirms()
        {
            Reload(ctx => ctx.Firms);
        }

        [Test]
        public void ReloadFirmBalances()
        {
            Reload(ctx => ctx.FirmBalances);
        }

        [Test]
        public void ReloadFirmCategories()
        {
            Reload(ctx => ctx.FirmCategories);
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(ctx => ctx.CategoryGroups);
        }

        [Test]
        public void ReloadClients()
        {
            Reload(ctx => ctx.Clients);
        }

        [Test]
        public void ReloadClientContacts()
        {
            Reload(ctx => ctx.ClientContacts);
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(ctx => ctx.Projects);
        }

        [Test]
        public void ReloadProjectCategories()
        {
            Reload(ctx => ctx.ProjectCategories);
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(ctx => ctx.Territories);
        }

        private void Reload<T>(Func<ICustomerIntelligenceContext, IEnumerable<T>> loader)
            where T : class
        {
            using (var factsDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var query = new Query(new StubReadableDomainContextProvider(factsDb.Connection, factsDb));
                var context = new CustomerIntelligenceTransformationContext(query);
                ciDb.Reload(loader(context));
            }
        }
    }
}