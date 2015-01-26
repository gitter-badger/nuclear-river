using System;
using System.Linq;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class CreateHelper
    {
        public IQueryable<T> CreateEntities<T>()
        {
            var queryable = Enumerable.Range(0, 10).Select(x => Activator.CreateInstance<T>()).AsQueryable();
            return queryable;
        }
    }
}