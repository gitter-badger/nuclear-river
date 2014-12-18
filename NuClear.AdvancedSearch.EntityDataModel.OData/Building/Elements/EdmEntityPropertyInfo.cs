namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmEntityPropertyInfo
    {
        public EdmEntityPropertyInfo(string name, EdmTypeReference typeReference)
        {
            Name = name;
            TypeReference = typeReference;
        }

        public string Name { get; private set; }

        public EdmTypeReference TypeReference { get; private set; }
    }
}