using System;
using System.Linq;

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
            Reload(query => Specs.Erm.Map.ToFacts.Accounts().Map(query));
        }

        [Test]
        public void ReloadBranchOfficeOrganizationUnits()
        {
            Reload(query => Specs.Erm.Map.ToFacts.BranchOfficeOrganizationUnits().Map(query));
        }

        [Test]
        public void ReloadCategories()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Categories().Map(query));
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(query => Specs.Erm.Map.ToFacts.CategoryGroups().Map(query));
        }

        [Test]
        public void ReloadCategoryFirmAddresses()
        {
            Reload(query => Specs.Erm.Map.ToFacts.CategoryFirmAddresses().Map(query));
        }

        [Test]
        public void ReloadCategoryOrganizationUnits()
        {
            Reload(query => Specs.Erm.Map.ToFacts.CategoryOrganizationUnits().Map(query));
        }

        [Test]
        public void ReloadClients()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Clients().Map(query));
        }

        [Test]
        public void ReloadContacts()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Contacts().Map(query));
        }

        [Test]
        public void ReloadFirms()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Firms().Map(query));
        }

        [Test]
        public void ReloadFirmAddresses()
        {
            Reload(query => Specs.Erm.Map.ToFacts.FirmAddresses().Map(query));
        }

        [Test]
        public void ReloadFirmContacts()
        {
            Reload(query => Specs.Erm.Map.ToFacts.FirmContacts().Map(query));
        }

        [Test]
        public void ReloadLegalPersons()
        {
            Reload(query => Specs.Erm.Map.ToFacts.LegalPersons().Map(query));
        }

        [Test]
        public void ReloadOrders()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Orders().Map(query));
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Projects().Map(query));
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(query => Specs.Erm.Map.ToFacts.Territories().Map(query));
        }

        private void Reload<T>(Func<IQuery, IQueryable<T>> loader)
            where T : class
        {
            using (var ermDb = CreateConnection("ErmSqlServer", Schema.Erm))
            using (var factDb = CreateConnection("FactsSqlServer", Schema.Facts))
            {
                var query = new Query(new StubReadableDomainContextProvider(ermDb.Connection, ermDb));
                factDb.Reload(loader(query));
            }
        }
    }
}