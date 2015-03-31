using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class FirmContact : IIdentifiable
    {
        public long Id { get; set; }

        public int ContactType { get; set; }

        public long? FirmAddressId { get; set; }
    }
}