using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public class EssentialViewFeature : IMetadataFeature
    {
        public EssentialViewFeature(string viewName)
        {
            ViewName = viewName;
        }

        public string ViewName { get; }
    }
}