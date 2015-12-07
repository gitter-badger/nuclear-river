namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class FirmContact : IErmFactObject
    {
        public long Id { get; set; }

        public bool HasPhone { get; set; }
        
        public bool HasWebsite { get; set; }

        public long FirmAddressId { get; set; }
    }
}