using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.EntityDataModel
{
    public sealed class EntityRelationCardinalityFeature : IUniqueMetadataFeature
    {
        public EntityRelationCardinalityFeature(Cardinality cardinality)
        {
            Cardinality = cardinality;
        }

        public Cardinality Cardinality { get; private set; }
    }
}