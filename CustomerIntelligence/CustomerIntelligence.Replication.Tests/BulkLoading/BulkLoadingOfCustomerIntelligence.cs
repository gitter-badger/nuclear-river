using System;
using System.Collections.Generic;
using System.Data.Common;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Replication.Tests.Data;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk.")]
    internal class BulkLoadingOfCustomerIntelligence : BulkLoadingFixtureBase
    {
        [Test]
        public void ReloadFirms()
        {
            Reload(query => Specs.Map.Facts.ToCI.Firms.Map(query));
        }

        [Test]
        public void ReloadFirmActivities()
        {
            Reload(query => Specs.Map.Facts.ToCI.FirmActivities.Map(query) );
        }

        [Test]
        public void ReloadFirmBalances()
        {
            Reload(query => Specs.Map.Facts.ToCI.FirmBalances.Map(query));
        }

        [Test]
        public void ReloadFirmCategories()
        {
            Reload(query => Specs.Map.Facts.ToCI.FirmCategories.Map(query));
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(query => Specs.Map.Facts.ToCI.CategoryGroups.Map(query));
        }

        [Test]
        public void ReloadClients()
        {
            Reload(query => Specs.Map.Facts.ToCI.Clients.Map(query));
        }

        [Test]
        public void ReloadClientContacts()
        {
            Reload(query => Specs.Map.Facts.ToCI.ClientContacts.Map(query));
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(query => Specs.Map.Facts.ToCI.Projects.Map(query));
        }

        [Test]
        public void ReloadProjectCategories()
        {
            Reload(query => Specs.Map.Facts.ToCI.ProjectCategories.Map(query));
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(query => Specs.Map.Facts.ToCI.Territories.Map(query));
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