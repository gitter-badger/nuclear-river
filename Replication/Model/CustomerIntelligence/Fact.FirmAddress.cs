namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class FirmAddress : IIdentifiable
        {
            public long Id { get; set; }
            public long FirmId { get; set; }

            public bool ClosedForAscertainment { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}