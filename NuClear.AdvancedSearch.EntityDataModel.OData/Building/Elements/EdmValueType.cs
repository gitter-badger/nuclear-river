using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmValueType : EdmStructuredType
    {
        public EdmValueType(string name, IReadOnlyCollection<EdmEntityPropertyInfo> properties)
            : base(name, properties)
        {
        }
    }
}