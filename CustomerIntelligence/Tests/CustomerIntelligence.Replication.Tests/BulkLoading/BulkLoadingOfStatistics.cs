using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Mapping;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Replication.Tests.Data;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Storage.API.Readings;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk."), Category("BulkStatistics")]
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
            using (var factDb = CreateConnection("FactsSqlServer", Schema.Facts))
            using (var ciDb = CreateConnection("CustomerIntelligenceSqlServer", Schema.CustomerIntelligence))
            {
                var annotation = ciDb.MappingSchema.GetAttributes<TableAttribute>(typeof(T)).Single();
                Console.WriteLine($"[{annotation.Schema}].[{annotation.Name ?? typeof(T).Name}]..");

                var query = new Query(new StubReadableDomainContextProvider((DbConnection)factDb.Connection, factDb));
                ciDb.Reload(loader(query), keyExpression);
            }
        }
    }
}