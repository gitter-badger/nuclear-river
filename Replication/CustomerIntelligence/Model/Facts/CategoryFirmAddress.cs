using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class CategoryFirmAddress : IIdentifiable
    {
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long FirmAddressId { get; set; }
    }
}