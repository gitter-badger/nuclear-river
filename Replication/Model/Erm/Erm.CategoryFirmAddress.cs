namespace NuClear.AdvancedSearch.Replication.Model.Erm
{
    public static partial class Erm
    {
        public sealed class CategoryFirmAddress : IEntity
        {
            public long Id { get; set; }
            public long CategoryId { get; set; }
            public long FirmAddressId { get; set; }
            
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}