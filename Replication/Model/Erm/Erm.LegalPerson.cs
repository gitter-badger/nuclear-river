namespace NuClear.AdvancedSearch.Replication.Model.Erm
{
    public static partial class Erm
    {
        public sealed class LegalPerson : IEntity
        {
            public long Id { get; set; }
            public long? ClientId { get; set; }
            
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}