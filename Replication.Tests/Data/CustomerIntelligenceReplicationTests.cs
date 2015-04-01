using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    [TestFixture, Explicit]
    internal class CustomerIntelligenceReplicationTests : BaseFixture
    {
        [Test]
        public void ReloadFirms()
        {
            Reload(ctx => ctx.Firms);
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
            using (var factsDb = FactsConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                var context = new CustomerIntelligenceTransformationContext(new FactsContext(factsDb));
                ciDb.Reload(loader(context));
            }
        }
    }
}