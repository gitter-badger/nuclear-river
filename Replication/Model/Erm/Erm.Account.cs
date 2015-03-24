namespace NuClear.AdvancedSearch.Replication.Model.Erm
{
    public static partial class Erm
    {
        public sealed class Account : IEntity
        {
            public long Id { get; set; }
            public decimal Balance { get; set; }
            public long LegalPersonId { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}