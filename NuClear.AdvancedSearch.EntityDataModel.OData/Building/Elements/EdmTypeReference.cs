namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmTypeReference
    {
        public EdmTypeReference(EdmType type)
            : this(type, true)
        {
        }

        public EdmTypeReference(EdmType type, bool isNullable)
        {
            Type = type;
            IsNullable = isNullable;
        }

        public bool IsNullable { get; private set; }

        public EdmType Type { get; private set; }
    }
}