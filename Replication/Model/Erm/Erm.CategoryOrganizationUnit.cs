namespace NuClear.AdvancedSearch.Replication.Model.Erm
{
    public static partial class Erm
    {
        public sealed class CategoryOrganizationUnit : IEntity
        {
            public long Id { get; set; }
            public long CategoryId { get; set; }
            public long CategoryGroupId { get; set; }
            public long OrganizationUnitId { get; set; }
            
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}