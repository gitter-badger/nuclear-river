using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    [TestFixture, Explicit("It's used to copy the data in bulk."), Category("BulkStatistics")]
    internal class BulkCopyingOfStatistics : BaseDataFixture
    {
        [Test]
        public void ReloadFirmCategoryStatistics()
        {
            Reload(ctx => ctx.FirmCategoryStatistics, x => new { x.FirmId, x.CategoryId });
        }

        private void Reload<T, TKey>(Func<IStatisticsContext, IEnumerable<T>> loader, Expression<Func<T, TKey>> keyExpression)
            where T : class
        {
            Console.WriteLine($"{typeof(T).Name}...");
            using (var factsDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var context = new StatisticsTransformationContext(new BitFactsContext(factsDb));
                ciDb.Reload(loader(context), keyExpression);
            }
        }
    }
}