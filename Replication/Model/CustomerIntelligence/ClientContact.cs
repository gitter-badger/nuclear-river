namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    internal sealed class ClientContact
    {
        public long Id { get; set; }
        public int Role { get; set; }
        public bool IsFired { get; set; }

        public long ClientId { get; set; }
    }
}