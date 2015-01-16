using System.Web.Http;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class DefaultController : ApiController
    {
        public string Get(int id)
        {
            return "Hello";
        }
    }
}