using System;
using System.Linq;
using System.Net.Http;
using System.Web.OData;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    internal static class ODataControllerExtensions
    {
        // workaround for https://github.com/OData/WebApi/issues/53
        public static void MakeCompatibleResponse(this ODataController controller, HttpResponseMessage response)
        {
            var compatibleParameter = controller.Request.Headers.Accept
                   .Where(x => string.Equals(x.MediaType, "application/json", StringComparison.OrdinalIgnoreCase))
                   .SelectMany(x => x.Parameters)
                   .FirstOrDefault(x => string.Equals(x.Name, "IEEE754Compatible", StringComparison.OrdinalIgnoreCase));

            if (compatibleParameter != null)
            {
                response.Content.Headers.ContentType.Parameters.Add(compatibleParameter);
            }
        }
    }
}