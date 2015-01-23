using System.Collections.Generic;
using System.Web.OData;
using System.Web.OData.Query;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public sealed class FirmsController : ODataController
    {
        public IEnumerable<object> Get(ODataQueryOptions options)
        {
            return new object[] { };
        }
    }
}