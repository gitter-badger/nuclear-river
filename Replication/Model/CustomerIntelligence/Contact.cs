namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    internal sealed class Contact
    {
        public long Id { get; set; }
        public int Role { get; set; }
        public bool IsFired { get; set; }

        public long ClientId { get; set; }
    }
}