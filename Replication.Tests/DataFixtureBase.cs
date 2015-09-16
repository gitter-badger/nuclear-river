using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Data;

using Moq;

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

        protected MockLinqToDbDataBuilder SourceDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected MockLinqToDbDataBuilder TargetDb
        {
            get { return _mockLinqToDbDataBuilder; }
        }

        protected IRepositoryFactory RepositoryFactory
        {
            get { return new LinqToDBRepositoryFactory(_stubDomainContextProvider); }
        }

        [Obsolete("Нужно удалить")]
        protected static IQueryable<T> Inquire<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }

        protected interface IRepositoryFactory
        {
            IRepository<T> Create<T>()
                where T : class;
        }

        protected class LinqToDBRepositoryFactory : IRepositoryFactory
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

        protected class VerifiableRepositoryFactory : IRepositoryFactory
        {
            private readonly IDictionary<Type, IRepository> _cache;

            public VerifiableRepositoryFactory()
            {
                _cache = new Dictionary<Type, IRepository>();
            }

            public IRepository<T> Create<T>() 
                where T : class
            {
                IRepository repository;
                if (!_cache.TryGetValue(typeof(T), out repository))
                {
                    var mock = new Mock<IRepository<T>>();
                    mock.Setup(x => x.AddRange(It.IsAny<IEnumerable<T>>()))
                        .Callback<IEnumerable<T>>(items => { foreach (var item in items) mock.Object.Add(item); });
                    mock.Setup(x => x.DeleteRange(It.IsAny<IEnumerable<T>>()))
                        .Callback<IEnumerable<T>>(items => { foreach (var item in items) mock.Object.Delete(item); });

                    _cache[typeof(T)] = repository = mock.Object;
                }

                return (IRepository<T>)repository;
            }

            public void Verify<T>(Expression<Action<IRepository<T>>> expression, Func<Times> times, string failMessage = null) 
                where T : class
            {
                var repository = (IRepository<T>)_cache[typeof(T)];
                var mock = Mock.Get(repository);
                mock.Verify(expression, times, failMessage);
            }
        }
    }
}