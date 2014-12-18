using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmEnumType : EdmType
    {
        public EdmEnumType(EdmPrimitiveTypeKind underlyingType, IReadOnlyDictionary<string, object> members)
        {
            UnderlyingType = underlyingType;
            Members = members;
        }

        public EdmPrimitiveTypeKind UnderlyingType { get; private set; }

        public IReadOnlyDictionary<string, object> Members { get; private set; }
    }
}