using System;
using System.Linq;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class StoreHelper
    {
        public IQueryable<T> GetEntities<T>()
        {
            var idProperty = typeof(T).GetProperty("Id");

            var queryable = Enumerable.Range(0, 10).Select(x =>
            {
                var entity = Activator.CreateInstance<T>();
                idProperty.SetValue(entity, x);
                return entity;
            }).AsQueryable();

            return queryable;
        }
    }
}