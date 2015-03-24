namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class CustomerIntelligence
    {
        public sealed class Client
        {
            public long Id { get; set; }
            
            public string Name { get; set; }

            public long CategoryGroupId { get; set; }
        }
    }
}