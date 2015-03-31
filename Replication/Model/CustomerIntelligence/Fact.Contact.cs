namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class Contact : IIdentifiable
        {
            public long Id { get; set; }

            public int Role { get; set; }

            public bool IsFired { get; set; }

            public bool HasPhone { get; set; }

            public bool HasWebsite { get; set; }

            public long ClientId { get; set; }
        }
    }
}