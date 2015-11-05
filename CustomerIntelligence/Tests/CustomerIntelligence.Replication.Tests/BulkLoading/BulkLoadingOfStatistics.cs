using System;
using System.Linq;
using System.Linq.Expressions;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk.")]
    public class BulkLoadingOfStatistics : BulkLoadingFixtureBase
    {
        public BulkLoadingOfStatistics()
            : this(new Loader("FactsSqlServer", Schema.Facts, "CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
        {
        }

        public BulkLoadingOfStatistics(ILoader loader)
            : base(loader)
        {
        }

        [Test]
        public void ReloadFirmCategoryStatistics()
        {
            Reload(query => Specs.Map.Facts.ToStatistics.FirmCategoryStatistics.Map(query), x => new { x.FirmId, x.CategoryId });
        }

        private void Reload<T>(Func<IQuery, IQueryable<T>> loader, Expression<Func<T, object>> key)
            where T : class
        {
            _loader.Reload(loader, key);
        }
    }
}