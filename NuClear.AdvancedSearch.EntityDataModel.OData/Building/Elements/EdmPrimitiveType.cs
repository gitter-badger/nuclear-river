namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmPrimitiveType : EdmType
    {
        public EdmPrimitiveType(EdmPrimitiveTypeKind typeKind)
        {
            TypeKind = typeKind;
        }

        public EdmPrimitiveTypeKind TypeKind { get; private set; }
    }
}