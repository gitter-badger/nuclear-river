namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class CategoryFirmAddress : IIdentifiable
        {
            public long Id { get; set; }

            public long CategoryId { get; set; }

            public long FirmAddressId { get; set; }
        }
    }
}