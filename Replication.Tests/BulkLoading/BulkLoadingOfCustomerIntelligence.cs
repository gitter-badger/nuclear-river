using System;
using System.Collections.Generic;
using System.Data.Common;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk.")]
    internal class BulkLoadingOfCustomerIntelligence : BulkLoadingFixtureBase
    {
        [Test]
        public void ReloadFirms()
        {
            Reload(query => Specs.Facts.Map.ToCI.Firms.Map(query));
        }

        [Test]
        public void ReloadFirmActivities()
        {
            Reload(query => Specs.Facts.Map.ToCI.FirmActivities.Map(query) );
        }

        [Test]
        public void ReloadFirmBalances()
        {
            Reload(query => Specs.Facts.Map.ToCI.FirmBalances.Map(query));
        }

        [Test]
        public void ReloadFirmCategories()
        {
            Reload(query => Specs.Facts.Map.ToCI.FirmCategories.Map(query));
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(query => Specs.Facts.Map.ToCI.CategoryGroups.Map(query));
        }

        [Test]
        public void ReloadClients()
        {
            Reload(query => Specs.Facts.Map.ToCI.Clients.Map(query));
        }

        [Test]
        public void ReloadClientContacts()
        {
            Reload(query => Specs.Facts.Map.ToCI.ClientContacts.Map(query));
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(query => Specs.Facts.Map.ToCI.Projects.Map(query));
        }

        [Test]
        public void ReloadProjectCategories()
        {
            Reload(query => Specs.Facts.Map.ToCI.ProjectCategories.Map(query));
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(query => Specs.Facts.Map.ToCI.Territories.Map(query));
        }

        private void Reload<T>(Func<IQuery, IEnumerable<T>> loader)
            where T : class
        {
            using (var factsDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var query = new Query(new StubReadableDomainContextProvider((DbConnection)factsDb.Connection, factsDb));
                ciDb.Reload(loader(query));
            }
        }
    }
}