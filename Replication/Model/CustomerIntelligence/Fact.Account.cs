namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class Account : IIdentifiable
        {
            public long Id { get; set; }

            public decimal Balance { get; set; }

            public long LegalPersonId { get; set; }
        }
    }
}