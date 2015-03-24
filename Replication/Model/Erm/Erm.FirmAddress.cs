namespace NuClear.AdvancedSearch.Replication.Model.Erm
{
    public static partial class Erm
    {
        public sealed class FirmAddress : IEntity
        {
            public long Id { get; set; }
            public long FirmId { get; set; }

            public bool ClosedForAscertainment { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}