using System;
using System.Data.Entity;
using System.Linq;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public interface IFinder
    {
        IQueryable<T> FindAll<T>() where T : class;
    }

    // TODO: очень проста€ имплементаци€, замутить через DbConfiguration/IDbDependencyResolver
    public class ODataFinder : IFinder, IDisposable
    {
        private readonly DbContext _context;

        public ODataFinder(ODataDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> FindAll<T>() where T : class
        {
            return _context.Set<T>().AsNoTracking();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}