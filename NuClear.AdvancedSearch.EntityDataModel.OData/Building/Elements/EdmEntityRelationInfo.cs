namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmEntityRelationInfo
    {
        public EdmEntityRelationInfo(string name, EdmEntityType sourceEntity, EdmEntityType targetEntity, EdmEntityRelationMultiplicity multiplicity)
        {
            Name = name;
            SourceEntity = sourceEntity;
            TargetEntity = targetEntity;
            TargetMultiplicity = multiplicity;
        }

        public string Name { get; private set; }

        public EdmEntityType SourceEntity { get; private set; }
        
        public EdmEntityType TargetEntity { get; private set; }

        public EdmEntityRelationMultiplicity TargetMultiplicity { get; private set; }
    }
}