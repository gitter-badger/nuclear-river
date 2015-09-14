using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    internal abstract class DataFixtureBase : FixtureBase
    {
        static DataFixtureBase()
        {
#if DEBUG
            //LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
            DataConnection.TurnTraceSwitchOn(TraceLevel.Verbose);
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1, s2);
#endif
        }

        private StubDomainContextProvider _stubDomainContextProvider;
        private MockLinqToDbDataBuilder _mockLinqToDbDataBuilder;

        [SetUp]
        public void FixtureBuildUp()
        {
            _stubDomainContextProvider = new StubDomainContextProvider();
            _mockLinqToDbDataBuilder = new MockLinqToDbDataBuilder(_stubDomainContextProvider);
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _stubDomainContextProvider.Dispose();
        }

        protected IQuery Query
        {
            get { return new Query(_stubDomainContextProvider); }
        }

        protected IFactChangesApplierFactory FactChangesApplierFactory
        {
            get { return new StubFactChangesApplierFactory(_stubDomainContextProvider); }
        }

        protected MockLinqToDbDataBuilder ErmDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected MockLinqToDbDataBuilder FactsDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected MockLinqToDbDataBuilder CustomerIntelligenceDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected static IQueryable<T> Inquire<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }
    }
}