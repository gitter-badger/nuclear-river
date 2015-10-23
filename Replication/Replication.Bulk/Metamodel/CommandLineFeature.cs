using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public class CommandLineFeature : IUniqueMetadataFeature
    {
        public CommandLineFeature(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}