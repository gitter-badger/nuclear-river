using System;
using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;

using NuClear.Storage.LinqToDB;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

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

        protected MockLinqToDbDataBuilder ErmDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected MockLinqToDbDataBuilder FactsDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected LinqToDBRepositoryFactory RepositoryFactory
        {
            get { return new LinqToDBRepositoryFactory(_stubDomainContextProvider); }
        }

        [Obsolete("Нужно удалить")]
        protected static IQueryable<T> Inquire<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }

        protected class LinqToDBRepositoryFactory
        {
            private readonly StubDomainContextProvider _stubDomainContextProvider;

            public LinqToDBRepositoryFactory(StubDomainContextProvider stubDomainContextProvider)
            {
                _stubDomainContextProvider = stubDomainContextProvider;
            }

            public IRepository<T> Create<T>() 
                where T : class
            {
                return new LinqToDBRepository<T>(_stubDomainContextProvider);
            }
        }
    }
}