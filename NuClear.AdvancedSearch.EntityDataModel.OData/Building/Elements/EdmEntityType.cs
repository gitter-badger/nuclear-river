using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmEntityType : EdmStructuredType
    {
        private readonly List<EdmEntityPropertyInfo> _keys = new List<EdmEntityPropertyInfo>();

        public EdmEntityType(string name, IReadOnlyCollection<EdmEntityPropertyInfo> properties)
            : base(name, properties)
        {
        }

        public IReadOnlyCollection<EdmEntityPropertyInfo> Keys { get { return _keys; } }
    }
}