using System.Linq;

using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.Storage.LinqToDB;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public sealed class MockLinqToDbDataBuilder
    {
        private readonly StubDomainContextProvider _stubDomainContextProvider;

        public MockLinqToDbDataBuilder(StubDomainContextProvider stubDomainContextProvider)
        {
            _stubDomainContextProvider = stubDomainContextProvider;
        }

        public MockLinqToDbDataBuilder Has<T>(T obj, params T[] moreObjects) where T : class
        {
            using (new NoSqlTrace())
            {
                var repository = new LinqToDBRepository<T>(_stubDomainContextProvider);
                repository.Add(obj);
                if (moreObjects != null && moreObjects.Any())
                {
                    repository.AddRange(moreObjects);
                }

                repository.Save();


                return this;
            }
        }
    }
}