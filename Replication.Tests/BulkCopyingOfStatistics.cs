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
    internal class BulkCopyingOfStatistics : BaseDataFixture
    {
        [Test]
        public void ReloadFirmCategoryStatistics()
        {
            Reload(ctx => ctx.FirmCategoryStatistics);
        }

        private void Reload<T>(Func<IStatisticsContext, IEnumerable<T>> loader)
            where T : class
        {
            using (var factsDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var context = new StatisticsTransformationContext(factsDb);
                ciDb.Reload(loader(context));
            }
        }
    }
}