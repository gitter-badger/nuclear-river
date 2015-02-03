using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

using NuClear.AdvancedSearch.Web.OData.DataAccess;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public abstract class GenericODataController<T> : ODataController where T : class
    {
        private readonly IFinder _finder;

        protected GenericODataController(IFinder finder)
        {
            _finder = finder;
        }

        public IHttpActionResult Get(ODataQueryOptions<T> queryOptions)
        {
            var entities = _finder.FindAll<T>();

            return Ok(entities);
        }
    }
}