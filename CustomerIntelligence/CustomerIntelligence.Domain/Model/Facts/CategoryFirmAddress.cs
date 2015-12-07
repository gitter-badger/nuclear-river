using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class CategoryFirmAddress : IFactObject
    {
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long FirmAddressId { get; set; }
    }
}