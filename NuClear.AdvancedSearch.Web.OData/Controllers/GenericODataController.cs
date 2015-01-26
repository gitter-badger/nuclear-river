using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class GenericODataController<T> : ODataController
    {
        public GenericODataController()
        {
            
        }

        [EnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<T> options)
        {
            var content = Enumerable.Empty<T>();

            return Ok(content);
        }
    }
}