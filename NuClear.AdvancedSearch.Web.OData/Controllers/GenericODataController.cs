using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class GenericODataController<T> : ODataController
    {
        private readonly CreateHelper _createHelper;

        public GenericODataController(CreateHelper createHelper)
        {
            _createHelper = createHelper;
        }

        [EnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<T> options)
        {
            var content = _createHelper.CreateEntities<T>();
            return Ok(content);
        }
    }
}