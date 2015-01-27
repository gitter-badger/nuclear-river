using System;
using System.Linq;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class StoreHelper
    {
        public IQueryable<T> GetEntities<T>()
        {
            var queryable = Enumerable.Range(0, 10).Select(x => Activator.CreateInstance<T>()).AsQueryable();
            return queryable;
        }
    }
}