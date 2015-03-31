using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class Contact : IIdentifiable
    {
        public long Id { get; set; }

        public int Role { get; set; }

        public bool IsFired { get; set; }

        public long ClientId { get; set; }
    }
}