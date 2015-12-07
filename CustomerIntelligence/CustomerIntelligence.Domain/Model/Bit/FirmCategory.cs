namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public sealed class FirmCategory : IBitFactObject
    {
        public long ProjectId { get; set; }
        public long FirmId { get; set; }
        public long CategoryId { get; set; }
    }
}