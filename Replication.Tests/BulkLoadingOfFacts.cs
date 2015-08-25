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
    internal class BulkLoadingOfFacts : BaseDataFixture
    {
        [Test]
        public void ReloadAccounts()
        {
            Reload(ctx => ctx.Accounts);
        }

        [Test]
        public void ReloadActivities()
        {
            Reload(ctx => ctx.Activities);
        }

        [Test]
        public void ReloadBranchOfficeOrganizationUnits()
        {
            Reload(ctx => ctx.BranchOfficeOrganizationUnits);
        }

        [Test]
        public void ReloadCategories()
        {
            Reload(ctx => ctx.Categories);
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(ctx => ctx.CategoryGroups);
        }

        [Test]
        public void ReloadCategoryFirmAddresses()
        {
            Reload(ctx => ctx.CategoryFirmAddresses);
        }

        [Test]
        public void ReloadCategoryOrganizationUnits()
        {
            Reload(ctx => ctx.CategoryOrganizationUnits);
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

        [Test]
        public void ReloadFirms()
        {
            Reload(ctx => ctx.Firms);
        }

        [Test]
        public void ReloadFirmAddresses()
        {
            Reload(ctx => ctx.FirmAddresses);
        }

        [Test]
        public void ReloadFirmContacts()
        {
            Reload(ctx => ctx.FirmContacts);
        }

        [Test]
        public void ReloadLegalPersons()
        {
            Reload(ctx => ctx.LegalPersons);
        }

        [Test]
        public void ReloadOrders()
        {
            Reload(ctx => ctx.Orders);
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(ctx => ctx.Projects);
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(ctx => ctx.Territories);
        }

        private void Reload<T>(Func<IErmFactsContext, IEnumerable<T>> loader)
            where T : class
        {
            using (var ermDb = CreateConnection("ErmSqlServer", Schema.Erm))
            using (var factDb = CreateConnection("FactsSqlServer", Schema.Facts))
            {
                var context = new ErmFactsTransformationContext(new ErmContext(ermDb));
                factDb.Reload(loader(context));
            }
        }
    }
}