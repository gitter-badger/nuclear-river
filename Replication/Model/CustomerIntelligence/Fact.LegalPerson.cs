namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class LegalPerson : IIdentifiable
        {
            public long Id { get; set; }
            public long? ClientId { get; set; }
        }
    }
}