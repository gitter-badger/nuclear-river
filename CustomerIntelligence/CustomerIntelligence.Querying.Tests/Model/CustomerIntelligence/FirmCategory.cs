namespace NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence
{
    public class FirmCategory
    {
        public long CategoryId { get; set; }
        
        public long FirmId { get; set; }

        public long? Hits { get; set; }

        public long? Shows { get; set; }
        
        public double? AdvertisersShare { get; set; }

        public long? FirmCount { get; set; }
    }
}