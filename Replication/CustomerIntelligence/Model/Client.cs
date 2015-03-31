using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class Client : IIdentifiable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CategoryGroupId { get; set; }
    }
}