using System;
using System.Linq;

namespace NuClear.AdvancedSearch.Web.OData.DataAccess
{
    public interface IFinder
    {
        IQueryable<T> FindAll<T>() where T : class;
    }

    public class ODataFinder : IFinder, IDisposable
    {
        private readonly ODataDbContext _context;

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