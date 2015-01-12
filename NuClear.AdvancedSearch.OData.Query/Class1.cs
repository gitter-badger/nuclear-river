using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;

using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.OData.Query
{
    public sealed class Class1
    {
        private IEdmModel

        public void Method1()
        {
            var model = (IEdmModel)null;
            var elementClrType = (Type)null;

            var context = new ODataQueryContext(model, elementClrType, null);

            var queryOptions = new Dictionary<string, string>
            {
                {"$skip", "1"}
            };
            var queryOptionParser = new ODataQueryOptionParser(context.Model, context.ElementType, context.NavigationSource, queryOptions);
            var filterQueryOption = new FilterQueryOption(null, context, queryOptionParser);

            filterQueryOption.ApplyTo();
        }
    }
}
