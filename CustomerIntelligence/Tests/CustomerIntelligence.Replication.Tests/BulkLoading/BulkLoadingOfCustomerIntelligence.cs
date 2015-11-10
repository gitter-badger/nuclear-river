using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk."), Category("BulkCustomerIntelligence")]
    public class BulkLoadingOfCustomerIntelligence : BulkLoadingFixtureBase
    {
        public BulkLoadingOfCustomerIntelligence()
            : this(new Loader("FactsSqlServer", Schema.Facts, "CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
        {
        }

        public BulkLoadingOfCustomerIntelligence(ILoader loader)
            : base(loader)
    {
        }

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
            _loader.Reload(loader);
        }
    }
}