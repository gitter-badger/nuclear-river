using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityRelationCardinalityFeature : IUniqueMetadataFeature
    {
        public EntityRelationCardinalityFeature(EntityRelationCardinality cardinality)
        {
            Cardinality = cardinality;
        }

        public EntityRelationCardinality Cardinality { get; private set; }
    }
}