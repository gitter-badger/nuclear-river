namespace NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence
{
    public class FirmCategory
    {
        public long CategoryId { get; set; }

        public long FirmId { get; set; }

        public int? Hits { get; set; }

        public int? Shows { get; set; }

        public double? AdvertisersShare { get; set; }

        public int? FirmCount { get; set; }
    }
}