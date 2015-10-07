using System;
using System.Linq;

using LinqToDB;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
    public sealed class LinqToDbQuery : IQuery
    {
        private readonly IDataContext _dataContext;

        public LinqToDbQuery(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable For(Type objType)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> For<T>() where T : class
        {
            return _dataContext.GetTable<T>();
        }

        public IQueryable<T> For<T>(FindSpecification<T> findSpecification) where T : class
        {
            return _dataContext.GetTable<T>().Where(findSpecification);
        }
    }
}