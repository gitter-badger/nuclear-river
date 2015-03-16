namespace NuClear.AdvancedSearch.Replication.Model.FactTable
{
    internal sealed class Contact
    {
        public long Id { get; set; }
        public int Role { get; set; }
        public bool IsFired { get; set; }
        public bool HasPhone { get; set; }
        public bool HasWebsite { get; set; }

        public long ClientId { get; set; }
    }
}