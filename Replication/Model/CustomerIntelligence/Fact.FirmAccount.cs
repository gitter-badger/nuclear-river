namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class FirmAccount
        {
            public long AccountId { get; set; }

            public long FirmId { get; set; }

            public decimal Balance { get; set; }
        }
    }
}