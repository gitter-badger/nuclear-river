using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public abstract class EdmStructuredType : EdmType
    {
        protected EdmStructuredType(string name, IReadOnlyCollection<EdmEntityPropertyInfo> properties)
        {
            Name = name;
            Properties = properties;
        }

        public string Name { get; private set; }

        public IReadOnlyCollection<EdmEntityPropertyInfo> Properties { get; private set; }
    }
}