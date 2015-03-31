namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class Client : IIdentifiable
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public bool HasPhone { get; set; }

            public bool HasWebsite { get; set; }
        }
    }
}