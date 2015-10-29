using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    [TestFixture, Explicit("It's used to copy the data in bulk."), Category("BulkCustomerIntelligence")]
    internal class BulkCopyingOfCustomerIntelligence : BaseDataFixture
    {
        [Test]
        public void ReloadFirms()
        {
            Reload(ctx => ctx.Firms);
        }

        [Test]
        public void ReloadFirmActivities()
        {
            Reload(ctx => ctx.FirmActivities);
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
        public void ReloadFirmTerritories()
        {
            Reload(ctx => ctx.FirmTerritories);
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
                var annotation = ciDb.MappingSchema.GetAttributes<TableAttribute>(typeof(T)).Single();
                Console.WriteLine($"[{annotation.Schema}].[{annotation.Name ?? typeof(T).Name}]..");

                var context = new CustomerIntelligenceTransformationContext(new ErmFactsContext(factsDb));
                ciDb.Reload(loader(context));
            }
        }
    }
}