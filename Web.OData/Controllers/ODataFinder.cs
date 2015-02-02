using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Web.OData.Extensions;

using NuClear.AdvancedSearch.QueryExecution;

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

        public ODataFinder(HttpRequestMessage request)
        {
            var dbCompiledModel = request.ODataProperties().Model.GetDbCompiledModel();
            _context = new DbContext("ODATA", dbCompiledModel);

            var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
            objectContext.CommandTimeout = 60;
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