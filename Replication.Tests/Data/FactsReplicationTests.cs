using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.Data.Context;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    [TestFixture, Explicit]
    internal class FactsReplicationTests : BaseFixture
    {
        [Test]
        public void ReloadAccounts()
        {
            Reload(ctx => ctx.Accounts);
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

        private void Reload<T>(Func<IFactsContext, IEnumerable<T>> loader)
        {
            using (var ermDb = ErmConnection)
            using (var factDb = FactsConnection)
            {
                var context = new FactsTransformationContext(new ErmContext(ermDb));
                factDb.Reload(loader(context));
            }
        }
    }
}