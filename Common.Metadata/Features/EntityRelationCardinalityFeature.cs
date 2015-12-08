using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    public sealed class EntityRelationCardinalityFeature : IUniqueMetadataFeature
    {
        public EntityRelationCardinalityFeature(EntityRelationCardinality cardinality, EntityElement target)
        {
            Cardinality = cardinality;
            Target = target;
        }

        public EntityRelationCardinality Cardinality { get; private set; }

        public EntityElement Target { get; private set; }
    }
}