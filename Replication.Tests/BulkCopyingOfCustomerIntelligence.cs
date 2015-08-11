using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.Specifications;
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
            Reload(query => Specs.Facts.Map.ToCI.Firms(null).Map(query));
        }

        [Test]
        public void ReloadFirmBalances()
        {
            Reload(query => Specs.Facts.Map.ToCI.FirmBalances(null).Map(query));
        }

        [Test]
        public void ReloadFirmCategories()
        {
            Reload(query => Specs.Facts.Map.ToCI.FirmCategories(null).Map(query));
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(query => Specs.Facts.Map.ToCI.CategoryGroups(null).Map(query));
        }

        [Test]
        public void ReloadClients()
        {
            Reload(query => Specs.Facts.Map.ToCI.Clients(null).Map(query));
        }

        [Test]
        public void ReloadClientContacts()
        {
            Reload(query => Specs.Facts.Map.ToCI.ClientContacts(null).Map(query));
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(query => Specs.Facts.Map.ToCI.Projects(null).Map(query));
        }

        [Test]
        public void ReloadProjectCategories()
        {
            Reload(query => Specs.Facts.Map.ToCI.ProjectCategories(null).Map(query));
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(query => Specs.Facts.Map.ToCI.Territories(null).Map(query));
        }

        private void Reload<T>(Func<IQuery, IEnumerable<T>> loader)
            where T : class
        {
            using (var factsDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var query = new Query(new StubReadableDomainContextProvider(factsDb.Connection, factsDb));
                ciDb.Reload(loader(query));
            }
        }
    }
}