namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class FirmContact : IIdentifiable
        {
            public long Id { get; set; }
            public int ContactType { get; set; }

            public long? FirmAddressId { get; set; }
        }
    }
}