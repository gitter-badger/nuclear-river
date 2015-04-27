using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.Tests.Data;

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
        public void ReloadFirmCategoryGroups()
        {
            Reload(ctx => ctx.FirmCategoryGroups);
        }

        [Test]
        public void ReloadClients()
        {
            Reload(ctx => ctx.Clients);
        }

        [Test]
        public void ReloadContacts()
        {
            Reload(ctx => ctx.Contacts);
        }

        private void Reload<T>(Func<ICustomerIntelligenceContext, IEnumerable<T>> loader)
        {
            using (var factsDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var context = new CustomerIntelligenceTransformationContext(new FactsContext(factsDb));
                ciDb.Reload(loader(context));
            }
        }
    }
}