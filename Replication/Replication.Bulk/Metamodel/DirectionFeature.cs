using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public class DirectionFeature : IUniqueMetadataFeature
    {
        public DirectionFeature(Storage source, Storage target)
        {
            Source = source;
            Target = target;
        }

        public Storage Source { get; }
        public Storage Target { get; }
    }
}