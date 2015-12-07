namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class CategoryFirmAddress : IErmFactObject
    {
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long FirmAddressId { get; set; }
    }
}