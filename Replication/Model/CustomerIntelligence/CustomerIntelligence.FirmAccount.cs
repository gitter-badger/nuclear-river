namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class CustomerIntelligence
    {
        public sealed class FirmAccount : IIdentifiable
        {
            public long Id { get; set; }
            public long AccountId { get; set; }
            public long FirmId { get; set; }
            public decimal Balance { get; set; }
        }
    }
}