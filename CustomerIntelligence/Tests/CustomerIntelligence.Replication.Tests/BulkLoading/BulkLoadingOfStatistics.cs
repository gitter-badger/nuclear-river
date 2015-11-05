using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Replication.Tests.Data;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Storage.API.Readings;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk.")]
    internal class BulkLoadingOfStatistics : BulkLoadingFixtureBase
    {
        [Test]
        public void ReloadFirmCategoryStatistics()
        {
            Reload(query => Specs.Map.Facts.ToStatistics.FirmCategoryStatistics.Map(query), x => new { x.FirmId, x.CategoryId });
        }

        private void Reload<T, TKey>(Func<IQuery, IQueryable<T>> loader, Expression<Func<T, TKey>> keyExpression)
            where T : class
        {
            using (var ermDb = CreateConnection("FactsSqlServer", Schema.Erm))
            using (var factDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.Facts))
            {
                var query = new Query(new StubReadableDomainContextProvider((DbConnection)ermDb.Connection, ermDb));
                factDb.Reload(loader(query), keyExpression);
            }
        }
    }
}