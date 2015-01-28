using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public abstract class GenericODataController<T> : ODataController
    {
        private readonly StoreHelper _storeHelper;

        protected GenericODataController(StoreHelper storeHelper)
        {
            _storeHelper = storeHelper;
        }

        public IHttpActionResult Get(ODataQueryOptions<T> queryOptions)
        {
            var entities = _storeHelper.GetEntities<T>();

            return Ok(entities);
        }
    }
}