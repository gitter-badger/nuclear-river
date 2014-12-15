using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmModelBuilder
    {
        public IEdmModel Build()
        {
            return new EdmModel();
        }
    }
}
