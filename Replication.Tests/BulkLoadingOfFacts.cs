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
    internal class BulkLoadingOfFacts : BaseDataFixture
    {
        [Test]
        public void ReloadAccounts()
        {
            Reload(query => query.For(Specs.Erm.Find.Accounts()));
        }

        [Test]
        public void ReloadBranchOfficeOrganizationUnits()
        {
            Reload(query => query.For(Specs.Erm.Find.BranchOfficeOrganizationUnits()));
        }

        [Test]
        public void ReloadCategories()
        {
            Reload(query => query.For(Specs.Erm.Find.Categories()));
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(query => query.For(Specs.Erm.Find.CategoryGroups()));
        }

        [Test]
        public void ReloadCategoryFirmAddresses()
        {
            Reload(query => query.For(Specs.Erm.Find.CategoryFirmAddresses()));
        }

        [Test]
        public void ReloadCategoryOrganizationUnits()
        {
            Reload(query => query.For(Specs.Erm.Find.CategoryOrganizationUnits()));
        }

        [Test]
        public void ReloadClients()
        {
            Reload(query => query.For(Specs.Erm.Find.Clients()));
        }

        [Test]
        public void ReloadContacts()
        {
            Reload(query => query.For(Specs.Erm.Find.Contacts()));
        }

        [Test]
        public void ReloadFirms()
        {
            Reload(query => query.For(Specs.Erm.Find.Firms()));
        }

        [Test]
        public void ReloadFirmAddresses()
        {
            Reload(query => query.For(Specs.Erm.Find.FirmAddresses()));
        }

        [Test]
        public void ReloadFirmContacts()
        {
            Reload(query => query.For(Specs.Erm.Find.FirmContacts()));
        }

        [Test]
        public void ReloadLegalPersons()
        {
            Reload(query => query.For(Specs.Erm.Find.LegalPersons()));
        }

        [Test]
        public void ReloadOrders()
        {
            Reload(query => query.For(Specs.Erm.Find.Orders()));
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(query => query.For(Specs.Erm.Find.Projects()));
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(query => query.For(Specs.Erm.Find.Territories()));
        }

        private void Reload<T>(Func<IQuery, IEnumerable<T>> loader)
            where T : class
        {
            using (var ermDb = CreateConnection("ErmSqlServer", Schema.Erm))
            using (var factDb = CreateConnection("FactsSqlServer", Schema.Facts))
            {
                var query = new Query(new StubReadableDomainContextProvider(ermDb));
                factDb.Reload(loader(query));
            }
        }
    }
}